namespace ProtoR.Web.Resources.GroupResource
{
    using ProtoR.Web.Infrastructure.Swagger;

    /// <summary>
    /// Group resource.
    /// </summary>
    public class GroupPutModel
    {
        /// <summary>
        /// Group name.
        /// </summary>
        [SwaggerExclude]
        public string GroupName { get; set; }

        /// <summary>
        /// ID of the category this group belongs to.
        /// </summary>
        public long CategoryId { get; set; }
    }
}
