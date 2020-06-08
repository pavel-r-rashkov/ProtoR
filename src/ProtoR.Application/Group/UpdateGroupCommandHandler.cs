namespace ProtoR.Application.Group
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class UpdateGroupCommandHandler : AsyncRequestHandler<UpdateGroupCommand>
    {
        private readonly IProtoBufSchemaGroupRepository schemaGroupRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserProvider userProvider;

        public UpdateGroupCommandHandler(
            IProtoBufSchemaGroupRepository schemaGroupRepository,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            this.schemaGroupRepository = schemaGroupRepository;
            this.unitOfWork = unitOfWork;
            this.userProvider = userProvider;
        }

        protected override async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await this.schemaGroupRepository.GetByName(request.GroupName);

            if (group == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>((object)request.GroupName);
            }

            var categories = this.userProvider.GetCategoryRestrictions();

            if (categories != null)
            {
                var hasAccessToNewCategory = categories.Contains(request.CategoryId);
                var hasAccessToOldCategory = categories.Contains(group.CategoryId);

                if (!hasAccessToNewCategory || !hasAccessToOldCategory)
                {
                    throw new InaccessibleCategoryException(
                        hasAccessToNewCategory ? group.CategoryId : request.CategoryId,
                        this.userProvider.GetCurrentUserName());
                }
            }

            group.CategoryId = request.CategoryId;

            await this.schemaGroupRepository.Update(group);
            await this.unitOfWork.SaveChanges();
        }
    }
}
