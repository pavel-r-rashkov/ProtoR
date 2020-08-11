namespace ProtoR.Application.Group
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application;
    using ProtoR.Application.Common;
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
            ProtoBufSchemaGroup group = await this.groupRepository.GetByName(request.Name);

            if (group == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>((object)request.Name);
            }

            var groupRestrictions = this.userProvider.GetGroupRestrictions();

            if (groupRestrictions != null)
            {
                var regex = FilterGenerator.CreateFromPatterns(groupRestrictions);
                var hasAccessToGroup = Regex.IsMatch(request.Name, regex, RegexOptions.IgnoreCase);

                if (!hasAccessToGroup)
                {
                    throw new InaccessibleGroupException(
                        request.Name,
                        this.userProvider.GetCurrentUserName());
                }
            }

            await this.groupRepository.Delete(group);
            await this.unitOfWork.SaveChanges();
        }
    }
}
