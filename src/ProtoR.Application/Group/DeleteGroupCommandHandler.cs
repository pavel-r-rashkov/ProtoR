namespace ProtoR.Application.Group
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class DeleteGroupCommandHandler : AsyncRequestHandler<DeleteGroupCommand>
    {
        private readonly IProtoBufSchemaGroupRepository groupRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserProvider userProvider;

        public DeleteGroupCommandHandler(
            IProtoBufSchemaGroupRepository groupRepository,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            this.groupRepository = groupRepository;
            this.unitOfWork = unitOfWork;
            this.userProvider = userProvider;
        }

        protected override async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            ProtoBufSchemaGroup group = await this.groupRepository.GetByName(request.GroupName);

            if (group == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>((object)request.GroupName);
            }

            var categories = this.userProvider.GetCategoryRestrictions();

            if (categories != null && !categories.Contains(group.CategoryId))
            {
                throw new InaccessibleCategoryException(
                    group.CategoryId,
                    this.userProvider.GetCurrentUserName());
            }

            await this.groupRepository.Delete(group);
            await this.unitOfWork.SaveChanges();
        }
    }
}
