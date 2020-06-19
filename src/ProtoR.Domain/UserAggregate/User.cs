namespace ProtoR.Domain.UserAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class User : Entity, IAggregateRoot
    {
        private List<RoleBinding> roleBindings;
        private IReadOnlyCollection<GroupRestriction> groupRestrictions;

        public User(string userName)
            : base(default)
        {
            this.UserName = userName;
            this.roleBindings = new List<RoleBinding>();
            this.groupRestrictions = new List<GroupRestriction> { new GroupRestriction("*") };
        }

        public User(
            long id,
            string userName,
            string normalizedUserName,
            string passwordHash,
            bool isActive,
            IReadOnlyCollection<GroupRestriction> groupRestrictions,
            IReadOnlyCollection<RoleBinding> roleBindings)
            : base(id)
        {
            this.UserName = userName;
            this.NormalizedUserName = normalizedUserName;
            this.PasswordHash = passwordHash;
            this.IsActive = isActive;
            this.GroupRestrictions = groupRestrictions;
            this.roleBindings = roleBindings.ToList();
        }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public bool IsActive { get; set; }

        public IReadOnlyCollection<GroupRestriction> GroupRestrictions
        {
            get
            {
                return this.groupRestrictions;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(this.GroupRestrictions));
                }

                if (!value.Any())
                {
                    throw new ArgumentException(nameof(this.GroupRestrictions));
                }

                this.groupRestrictions = value;
            }
        }

        public IReadOnlyCollection<RoleBinding> RoleBindings { get => this.roleBindings; }

        public void AddRole(long roleId)
        {
            var newRoleBinding = new RoleBinding(roleId, this.Id, null);

            if (!this.RoleBindings.Contains(newRoleBinding))
            {
                this.roleBindings.Add(newRoleBinding);
            }
        }

        public void RemoveRole(long roleId)
        {
            var roleBinding = new RoleBinding(roleId, this.Id, null);
            this.roleBindings.Remove(roleBinding);
        }

        public void SetRoles(IEnumerable<long> roleIds)
        {
            this.roleBindings = roleIds
                .Select(r => new RoleBinding(r, this.Id, null))
                .ToList();
        }
    }
}
