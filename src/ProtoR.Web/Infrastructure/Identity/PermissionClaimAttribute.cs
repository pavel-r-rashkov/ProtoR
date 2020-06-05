namespace ProtoR.Web.Infrastructure.Identity
{
    using System;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionClaimAttribute : Attribute, IFilterFactory
    {
        private readonly Permission permission;

        public PermissionClaimAttribute(Permission permission)
        {
            this.permission = permission;
        }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = (PermissionClaimFilter)serviceProvider.GetService(typeof(PermissionClaimFilter));
            filter.Permission = this.permission;

            return filter;
        }
    }
}
