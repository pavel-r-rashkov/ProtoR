namespace ProtoR.Web.Resources.RoleResource
{
    using System.Collections.Generic;
    using HybridModelBinding;
    using ProtoR.Web.Infrastructure.Swagger;

    public class RoleWriteModel
    {
        [SwaggerExclude]
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<int> Permissions { get; set; }
    }
}
