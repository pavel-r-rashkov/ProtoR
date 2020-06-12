namespace ProtoR.Web.Resources.UserResource
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// User resource.
    /// </summary>
    public class UserReadModel : ICreationInfo
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// List of role IDs associated with this user.
        /// </summary>
        public IEnumerable<long> RoleBindings { get; set; }

        /// <summary>
        /// List of category IDs associated with this user.
        /// </summary>
        public IEnumerable<long> CategoryBindings { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public string CreatedBy { get; set; }
    }
}
