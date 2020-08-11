namespace ProtoR.Web.Resources.SchemaResource
{
    using ProtoR.Web.Infrastructure.Swagger;

    /// <summary>
    /// Schema resource.
    /// </summary>
    public class SchemaWriteModel
    {
        /// <summary>
        /// Group name.
        /// </summary>
        [SwaggerExclude]
        public string Name { get; set; }

        /// <summary>
        /// Schema contents.
        /// </summary>
        public string Contents { get; set; }
    }
}
