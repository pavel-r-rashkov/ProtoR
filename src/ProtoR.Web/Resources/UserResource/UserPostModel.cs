namespace ProtoR.Web.Resources.UserResource
{
    using System.Collections.Generic;

    /// <summary>
    /// User resource.
    /// </summary>
    public class UserPostModel
    {
        /// <summary>
        /// User name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Is user active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// List of role IDs associated with this user.
        /// </summary>
        public IEnumerable<long> Roles { get; set; }

        /// <summary>
        /// List of group restriction patterns associated with this user.
        /// </summary>
        public IEnumerable<string> GroupRestrictions { get; set; }
    }
}
