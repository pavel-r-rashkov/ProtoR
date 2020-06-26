namespace ProtoR.Application.Role
{
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SeedWork;
    using Permission = ProtoR.Domain.RoleAggregate.Permission;
    using Role = ProtoR.Domain.RoleAggregate.Role;

    public class UpdateRoleCommandHandler : AsyncRequestHandler<UpdateRoleCommand>
    {
        private readonly RoleManager<Role> roleManager;
        private readonly IUnitOfWork unitOfWork;

        public UpdateRoleCommandHandler(
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork)
        {
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            var role = await this.roleManager.FindByIdAsync(command.Id.ToString(CultureInfo.InvariantCulture));
            role.Name = command.Name;
            role.AssignPermissions(command.Permissions
                .Select(p => Permission.FromId(p))
                .ToList());

            var result = await this.roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new RoleException("Identity error during role update", result.IdentityErrors());
            }

            await this.unitOfWork.SaveChanges();
        }
    }
}
