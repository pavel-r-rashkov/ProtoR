namespace ProtoR.Web.Resources.RoleResource
{
    using System.Collections.Generic;
    using ProtoR.Web.Infrastructure.Swagger;

    /// <summary>
    /// Role resource.
    /// </summary>
    public class RoleWriteModel
    {
        /// <summary>
        /// Role ID.
        /// </summary>
        [SwaggerExclude]
        public int Id { get; set; }

        /// <summary>
        /// Role name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of permission IDs associated with this role.
        /// </summary>
        public IEnumerable<int> Permissions { get; set; }
    }
}
