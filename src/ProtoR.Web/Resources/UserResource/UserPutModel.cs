namespace ProtoR.Web.Resources.UserResource
{
    using System.Collections.Generic;
    using ProtoR.Web.Infrastructure.Swagger;

    public class UserPutModel
    {
        [SwaggerExclude]
        public long Id { get; set; }

        public IEnumerable<long> Roles { get; set; }

        public IEnumerable<long> Categories { get; set; }
    }
}
