namespace ProtoR.Web.Resources.CategoryResource
{
    using ProtoR.Web.Infrastructure.Swagger;

    /// <summary>
    /// Category resource.
    /// </summary>
    public class CategoryWriteModel
    {
        /// <summary>
        /// Category ID.
        /// </summary>
        [SwaggerExclude]
        public long Id { get; set; }

        /// <summary>
        /// Category name.
        /// </summary>
        public string Name { get; set; }
    }
}
