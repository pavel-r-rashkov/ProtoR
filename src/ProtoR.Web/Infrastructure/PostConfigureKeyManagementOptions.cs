namespace ProtoR.Web.Infrastructure
{
    using Microsoft.AspNetCore.DataProtection.KeyManagement;
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using Microsoft.Extensions.Options;
    using ProtoR.Infrastructure.DataAccess;

    public class PostConfigureKeyManagementOptions : IPostConfigureOptions<KeyManagementOptions>
    {
        private readonly IXmlRepository xmlRepository;
        private readonly IIgniteFactory igniteFactory;

        public PostConfigureKeyManagementOptions(
            IXmlRepository xmlRepository,
            IIgniteFactory igniteFactory)
        {
            this.xmlRepository = xmlRepository;
            this.igniteFactory = igniteFactory;
        }

        public void PostConfigure(string name, KeyManagementOptions options)
        {
            this.igniteFactory.InitalizeIgnite();
            options.XmlRepository = this.xmlRepository;
        }
    }
}
