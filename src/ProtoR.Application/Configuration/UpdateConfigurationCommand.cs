namespace ProtoR.Application.Configuration
{
    using System.Collections.Generic;
    using MediatR;

    public class UpdateConfigurationCommand : IRequest
    {
        public string ConfigurationId { get; set; }

        public bool Transitive { get; set; }

        public bool ForwardCompatible { get; set; }

        public bool BackwardCompatible { get; set; }

        public bool Inherit { get; set; }

        public IEnumerable<RuleConfigurationDto> RuleConfigurations { get; set; }
    }
}
