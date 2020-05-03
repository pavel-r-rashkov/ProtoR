namespace ProtoR.Application.Group
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class DeleteGroupCommandHandler : AsyncRequestHandler<DeleteGroupCommand>
    {
        private readonly IProtoBufSchemaGroupRepository groupRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteGroupCommandHandler(
            IProtoBufSchemaGroupRepository groupRepository,
            IUnitOfWork unitOfWork)
        {
            this.groupRepository = groupRepository;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            ProtoBufSchemaGroup group = await this.groupRepository.GetByName(request.GroupName);
            await this.groupRepository.Delete(group);
            await this.unitOfWork.SaveChanges();
        }
    }
}
