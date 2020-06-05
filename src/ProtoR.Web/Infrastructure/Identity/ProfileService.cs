namespace ProtoR.Web.Infrastructure.Identity
{
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Identity;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.UserAggregate;

    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> userManager;
        private readonly IRoleRepository roleRepository;

        public ProfileService(
            UserManager<User> userManager,
            IRoleRepository roleRepository)
        {
            this.userManager = userManager;
            this.roleRepository = roleRepository;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await this.userManager.GetUserAsync(context.Subject);
            var roles = await this.roleRepository.GetRoles(user.RoleBindings.Select(r => r.RoleId));
            var permissions = roles.SelectMany(r => r.Permissions).Select(p => p.Id).Distinct();
            var categories = user.CategoryBindings.Select(c => c.CategoryId);

            context.IssuedClaims.AddRange(CustomClaim.ForPermissions(permissions));
            context.IssuedClaims.AddRange(CustomClaim.ForCategories(categories));
            context.IssuedClaims.Add(CustomClaim.ForUserName(user.UserName));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await this.userManager.GetUserAsync(context.Subject);
            context.IsActive = user != null;
        }
    }
}
