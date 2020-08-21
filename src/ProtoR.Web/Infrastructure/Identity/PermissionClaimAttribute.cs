namespace ProtoR.Web.Infrastructure.Identity
{
    using System;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionClaimAttribute : Attribute, IFilterFactory
    {
        public PermissionClaimAttribute(Permission permission)
        {
            this.Permission = permission;
        }

        public bool IsReusable => false;

        public Permission Permission { get; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = (PermissionClaimFilter)serviceProvider.GetService(typeof(PermissionClaimFilter));
            filter.Permission = this.Permission;

            return filter;
        }
    }
}
