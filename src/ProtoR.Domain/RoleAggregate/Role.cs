namespace ProtoR.Domain.RoleAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.SeedWork;

    public class Role : Entity, IAggregateRoot
    {
        private IEnumerable<Permission> permissions;
        private string name;
        private string normalizedName;

        public Role(
            string name,
            IEnumerable<Permission> permissions)
            : base(default)
        {
            this.Name = name;
            this.permissions = permissions ?? new List<Permission>();
        }

        public Role(
            long id,
            string name,
            string normalizedName,
            IEnumerable<Permission> permissions)
            : base(id)
        {
            this.Name = name;
            this.NormalizedName = normalizedName;
            this.permissions = permissions ?? new List<Permission>();
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.Name)} cannot be null or white space");
                }

                this.name = value;
            }
        }

        public string NormalizedName
        {
            get
            {
                return this.normalizedName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.NormalizedName)} cannot be null or white space");
                }

                this.normalizedName = value;
            }
        }

        public IReadOnlyCollection<Permission> Permissions
        {
            get
            {
                return this.permissions.ToList();
            }
        }

        public void AssignPermissions(IList<Permission> permissions)
        {
            this.permissions = permissions.Distinct().AsEnumerable();
        }
    }
}
