namespace ProtoR.Application.User
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;
    using User = ProtoR.Domain.UserAggregate.User;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, long>
    {
        private readonly UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;

        public CreateUserCommandHandler(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var newUser = new User(command.UserName)
            {
                IsActive = command.IsActive,
            };

            newUser.SetRoles(command.Roles ?? Array.Empty<long>());
            newUser.GroupRestrictions = command.GroupRestrictions
                .Select(pattern => new GroupRestriction(pattern))
                .ToList();

            var result = await this.userManager.CreateAsync(newUser, command.Password);
            await this.unitOfWork.SaveChanges();

            if (!result.Succeeded)
            {
                throw new UserException("Identity error during user creation", result.IdentityErrors());
            }

            var user = await this.userManager.FindByNameAsync(command.UserName);

            return user.Id;
        }
    }
}
