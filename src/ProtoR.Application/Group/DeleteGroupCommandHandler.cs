namespace ProtoR.Application.Group
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application;
    using ProtoR.Domain.CategoryAggregate;
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
            ProtoBufSchemaGroup group = await this.groupRepository.GetByName(request.GroupName);
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
