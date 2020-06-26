namespace ProtoR.Application.User
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;
    using User = ProtoR.Domain.UserAggregate.User;

    public class UpdateUserCommandHandler : AsyncRequestHandler<UpdateUserCommand>
    {
        private readonly UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;

        public UpdateUserCommandHandler(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await this.userManager.FindByIdAsync(command.Id.ToString(CultureInfo.InvariantCulture));

            if (user == null)
            {
                throw new EntityNotFoundException<User>(command.Id);
            }

            user.IsActive = command.IsActive;
            user.SetRoles(command.Roles ?? Array.Empty<long>());
            user.GroupRestrictions = command.GroupRestrictions
                .Select(pattern => new GroupRestriction(pattern))
                .ToList();

            if (command.NewPassword != null)
            {
                var passwordChangeResult = await this.userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);

                if (!passwordChangeResult.Succeeded)
                {
                    throw new UserException("Identity error during user password change", passwordChangeResult.IdentityErrors());
                }
            }

            var result = await this.userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new UserException("Identity error during user update", result.IdentityErrors());
            }

            await this.unitOfWork.SaveChanges();
        }
    }
}
