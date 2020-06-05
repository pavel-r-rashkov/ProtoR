namespace ProtoR.Web.Infrastructure.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.UserAggregate;

    public sealed class UserStore : IUserRoleStore<User>, IUserPasswordStore<User>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;

        public UserStore(
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await this.roleRepository.GetByName(roleName);
            user.AddRole(role.Id);
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = await this.FindByNameAsync(user.NormalizedUserName, cancellationToken);

            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "existingUser",
                    Description = "User with this name already exists",
                });
            }

            await this.userRepository.Add(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await this.userRepository.Delete(user.Id);

            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var id = Convert.ToInt64(userId, CultureInfo.InvariantCulture);

            return await this.userRepository.GetById(id);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await this.userRepository.GetByName(normalizedUserName);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            var roleNames = (await this.roleRepository
                .GetRoles(user.RoleBindings.Select(r => r.RoleId)))
                .Select(r => r.Name)
                .ToList();

            return roleNames;
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            var userId = user.Id.ToString(CultureInfo.InvariantCulture);

            return Task.FromResult(userId);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = await this.roleRepository.GetByName(roleName);
            return (await this.userRepository
                .GetUsersInRole(role.Id))
                .ToList();
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await this.roleRepository.GetByName(roleName);

            return user.RoleBindings.Any(r => r.RoleId == role.Id);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await this.roleRepository.GetByName(roleName);
            user.RemoveRole(role.Id);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await this.userRepository.Update(user);

            return IdentityResult.Success;
        }
    }
}
