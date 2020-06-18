namespace ProtoR.Web.Infrastructure.Identity
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;

    public static class CustomClaim
    {
        public static readonly string PermissionClaimType = $"{ClaimsPrefix}permission";
        public static readonly string GroupRestrictionClaimType = $"{ClaimsPrefix}group_restriction";
        public static readonly string UserNameClaimType = $"{ClaimsPrefix}username";
        public static readonly string ClientNameClaimType = $"{ClaimsPrefix}client_name";
        private const string ClaimsPrefix = "protor_";

        public static IEnumerable<Claim> ForPermissions(IEnumerable<int> permissions)
        {
            return permissions.Select(p => new Claim(PermissionClaimType, p.ToString(CultureInfo.InvariantCulture)));
        }

        public static IEnumerable<Claim> ForGroupRestrictions(IEnumerable<string> groupRestrictions)
        {
            return groupRestrictions.Select(c => new Claim(GroupRestrictionClaimType, c.ToString(CultureInfo.InvariantCulture)));
        }

        public static Claim ForUserName(string userName)
        {
            return new Claim(UserNameClaimType, userName);
        }

        public static Claim ForClientName(string clientName)
        {
            return new Claim(ClientNameClaimType, clientName);
        }
    }
}
