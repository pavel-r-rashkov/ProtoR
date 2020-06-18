namespace ProtoR.Web.Resources.UserResource
{
    using System.Collections.Generic;
    using ProtoR.Web.Infrastructure.Swagger;

    /// <summary>
    /// User resource.
    /// </summary>
    public class UserPutModel
    {
        /// <summary>
        /// User ID.
        /// </summary>
        [SwaggerExclude]
        public long Id { get; set; }

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
