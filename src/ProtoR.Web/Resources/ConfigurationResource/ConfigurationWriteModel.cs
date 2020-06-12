namespace ProtoR.Web.Resources.ConfigurationResource
{
    using System.Collections.Generic;
    using ProtoR.Web.Infrastructure.Swagger;

    /// <summary>
    /// Configuration resource.
    /// </summary>
    public class ConfigurationWriteModel
    {
        /// <summary>
        /// Configuration ID.
        /// </summary>
        [SwaggerExclude]
        public string ConfigurationId { get; set; }

        /// <summary>
        /// When true all schemas in a group are checked for compatiblity when inserting a new schema.
        /// When false only the latest schema in the group is checked for compatiblity when inserting a new schema.
        /// </summary>
        public bool Transitive { get; set; }

        /// <summary>
        /// When true rules evaluation will use the older schema as base.
        /// E.g. appending a new schema V2 to a group with existing schema V1,
        /// where V2 contains a message that doesn't exist in V1, will trigger "Message added" rule.
        /// When false no backward compatiblity checks are performed.
        /// At least one of <see cref="BackwardCompatible"/> and <see cref="ForwardCompatible"/> is required.
        /// </summary>
        public bool BackwardCompatible { get; set; }

        /// <summary>
        /// When true rules evaluation will use the new schema as base.
        /// E.g. appending a new schema V2 to a group with existing schema V1,
        /// where V2 contains a message that doesn't exist in V1, will trigger "Message removed" rule.
        /// When false no forward compatiblity checks are performed.
        /// At least one of <see cref="BackwardCompatible"/> and <see cref="ForwardCompatible"/> is required.
        /// </summary>
        public bool ForwardCompatible { get; set; }

        /// <summary>
        /// When true the values of <see cref="BackwardCompatible"/>, <see cref="ForwardCompatible"/> and <see cref="Transitive"/> are taken from the global configuration.
        /// </summary>
        public bool Inherit { get; set; }

        /// <summary>
        /// Rule configurations.
        /// </summary>
        public IEnumerable<RuleConfigurationModel> RuleConfigurations { get; set; }
    }
}
