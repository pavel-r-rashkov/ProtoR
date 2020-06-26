namespace ProtoR.Application.Role
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SeedWork;
    using Role = ProtoR.Domain.RoleAggregate.Role;

    public class DeleteRoleCommandHandler : AsyncRequestHandler<DeleteRoleCommand>
    {
        private readonly RoleManager<Role> roleManager;
        private readonly IUnitOfWork unitOfWork;

        public DeleteRoleCommandHandler(
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork)
        {
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
        {
            var role = await this.roleManager.FindByIdAsync(command.RoleId.ToString(CultureInfo.InvariantCulture));

            if (role == null)
            {
                throw new EntityNotFoundException<Role>(command.RoleId);
            }

            var result = await this.roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                throw new RoleException("Identity error during role deletion", result.IdentityErrors());
            }

            await this.unitOfWork.SaveChanges();
        }
    }
}
