namespace ProtoR.Web.Resources.CategoryResource
{
    using ProtoR.Web.Infrastructure.Swagger;

    public class CategoryWriteModel
    {
        [SwaggerExclude]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
