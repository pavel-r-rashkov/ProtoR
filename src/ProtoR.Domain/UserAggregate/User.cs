namespace ProtoR.Domain.UserAggregate
{
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SeedWork;

    public class User : Entity, IAggregateRoot
    {
        private List<RoleBinding> roleBindings;
        private List<CategoryBinding> categoryBindings;

        public User(string userName)
            : base(default)
        {
            this.UserName = userName;
            this.roleBindings = new List<RoleBinding>();
            this.categoryBindings = new List<CategoryBinding>();
        }

        public User(
            long id,
            string userName,
            string normalizedUserName,
            string passwordHash,
            IReadOnlyCollection<RoleBinding> roleBindings,
            IReadOnlyCollection<CategoryBinding> categoryBindings)
            : base(id)
        {
            this.UserName = userName;
            this.NormalizedUserName = normalizedUserName;
            this.PasswordHash = passwordHash;
            this.roleBindings = roleBindings.ToList();
            this.categoryBindings = categoryBindings.ToList();
        }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public IReadOnlyCollection<RoleBinding> RoleBindings { get => this.roleBindings; }

        public IReadOnlyCollection<CategoryBinding> CategoryBindings { get => this.categoryBindings; }

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

        public void SetCategories(IEnumerable<long> categoryIds)
        {
            this.categoryBindings = categoryIds
                .Select(r => new CategoryBinding(r, this.Id, null))
                .ToList();
        }
    }
}
