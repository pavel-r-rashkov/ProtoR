namespace ProtoR.Web.Infrastructure.Identity
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.RoleAggregate;

    public sealed class RoleStore : IRoleStore<Role>
    {
        private readonly IRoleRepository roleRepository;

        public RoleStore(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            await this.roleRepository.Add(role);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            await this.roleRepository.Delete(role.Id);

            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var id = Convert.ToInt64(roleId, CultureInfo.InvariantCulture);

            return await this.roleRepository.GetById(id);
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return await this.roleRepository.GetByName(normalizedRoleName);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            var roleId = role.Id.ToString(CultureInfo.InvariantCulture);

            return Task.FromResult(roleId);
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            await this.roleRepository.Update(role);

            return IdentityResult.Success;
        }
    }
}
