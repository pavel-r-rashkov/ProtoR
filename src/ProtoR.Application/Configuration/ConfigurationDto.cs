namespace ProtoR.Application.Configuration
{
    using System.Collections.Generic;

    public class ConfigurationDto
    {
        public long Id { get; set; }

        public long? GroupId { get; set; }

        public bool Transitive { get; set; }

        public bool ForwardCompatible { get; set; }

        public bool BackwardCompatible { get; set; }

        public IEnumerable<RuleConfigurationDto> RuleConfigurations { get; set; }
    }
}
