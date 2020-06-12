namespace ProtoR.Web.Resources.RoleResource
{
    using System;
    using System.Collections.Generic;

    public class RoleReadModel : ICreationInfo
    {
        /// <summary>
        /// Role ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Role name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of permission IDs associated with this role.
        /// </summary>
        public IEnumerable<int> Permissions { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public string CreatedBy { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public DateTime CreatedOn { get; set; }
    }
}
