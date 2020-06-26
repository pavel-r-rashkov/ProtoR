namespace ProtoR.Application.Role
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SeedWork;
    using Permission = ProtoR.Domain.RoleAggregate.Permission;
    using Role = ProtoR.Domain.RoleAggregate.Role;

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, long>
    {
        private readonly RoleManager<Role> roleManager;
        private readonly IUnitOfWork unitOfWork;

        public CreateRoleCommandHandler(
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork)
        {
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
        {
            var role = new Role(command.Name, command.Permissions
                .Select(p => Permission.FromId(p))
                .ToList());

            var result = await this.roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new RoleException("Identity error during role creation", result.IdentityErrors());
            }

            await this.unitOfWork.SaveChanges();
            var newRole = await this.roleManager.FindByNameAsync(role.Name);

            return newRole.Id;
        }
    }
}
