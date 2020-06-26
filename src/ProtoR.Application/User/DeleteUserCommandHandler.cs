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
    using ProtoR.Domain.SeedWork;
    using User = ProtoR.Domain.UserAggregate.User;

    public class DeleteUserCommandHandler : AsyncRequestHandler<DeleteUserCommand>
    {
        private readonly UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;

        public DeleteUserCommandHandler(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var user = await this.userManager.FindByIdAsync(command.UserId.ToString(CultureInfo.InvariantCulture));

            if (user == null)
            {
                throw new EntityNotFoundException<User>(command.UserId);
            }

            var result = await this.userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new UserException("Identity error during user deletion", result.IdentityErrors());
            }

            await this.unitOfWork.SaveChanges();
        }
    }
}
