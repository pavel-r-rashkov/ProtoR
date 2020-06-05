namespace ProtoR.Domain.RoleAggregate
{
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.SeedWork;

    public class Permission : Enumeration
    {
        private static Dictionary<int, Permission> allPermissions;

        protected Permission(int id, string name)
            : base(id, name)
        {
        }

        public static Permission GroupRead { get; } = new Permission(1, "Group Read");

        public static Permission GroupWrite { get; } = new Permission(2, "Group Write");

        public static Permission SchemaRead { get; } = new Permission(3, "Schema Read");

        public static Permission SchemaWrite { get; } = new Permission(4, "Schema Write");

        public static Permission ConfigurationRead { get; } = new Permission(5, "Configuration Read");

        public static Permission ConfigurationWrite { get; } = new Permission(6, "Configuration Write");

        public static Permission UserRead { get; } = new Permission(7, "User Read");

        public static Permission UserWrite { get; } = new Permission(8, "User Write");

        public static Permission ClientRead { get; } = new Permission(9, "Client Read");

        public static Permission ClientWrite { get; } = new Permission(10, "Client Write");

        public static Permission CategoryRead { get; } = new Permission(11, "Category Read");

        public static Permission CategoryWrite { get; } = new Permission(12, "Category Write");

        public static Permission RoleRead { get; } = new Permission(13, "Role Read");

        public static Permission RoleWrite { get; } = new Permission(14, "Role Write");

        public static Permission PermissionRead { get; } = new Permission(15, "Permission Read");

        public static Permission FromId(int id)
        {
            if (allPermissions == null)
            {
                allPermissions = GetAll<Permission>().ToDictionary(p => p.Id);
            }

            if (!allPermissions.TryGetValue(id, out var permission))
            {
                throw new KeyNotFoundException($"There is no permssion with id {id}");
            }

            return permission;
        }
    }
}
