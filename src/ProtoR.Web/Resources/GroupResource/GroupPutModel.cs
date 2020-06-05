namespace ProtoR.Web.Resources.GroupResource
{
    using ProtoR.Web.Infrastructure.Swagger;

    public class GroupPutModel
    {
        [SwaggerExclude]
        public string GroupName { get; set; }

        public long CategoryId { get; set; }
    }
}
