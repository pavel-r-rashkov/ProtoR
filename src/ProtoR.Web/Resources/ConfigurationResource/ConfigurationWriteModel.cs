namespace ProtoR.Web.Resources.ConfigurationResource
{
    using System.Collections.Generic;
    using ProtoR.Web.Infrastructure.Swagger;

    public class ConfigurationWriteModel
    {
        [SwaggerExclude]
        public string ConfigurationId { get; set; }

        public bool Transitive { get; set; }

        public bool BackwardCompatible { get; set; }

        public bool ForwardCompatible { get; set; }

        public bool Inherit { get; set; }

        public IEnumerable<RuleConfigurationModel> RuleConfigurations { get; set; }
    }
}
