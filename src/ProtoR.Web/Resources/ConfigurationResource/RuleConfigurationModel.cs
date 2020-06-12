namespace ProtoR.Web.Resources.ConfigurationResource
{
    /// <summary>
    /// Rule configuration.
    /// </summary>
    public class RuleConfigurationModel
    {
        /// <summary>
        /// Rule code. Accepted values: PB0001 - PB0020.
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// Severity of violations for this rule.
        /// 1 - Hidden - this rule is skipped during schema evaluation.
        /// 2 - Info - rule violations are reported to the user as information. Schemas are inserted in the group despite violations with Info severity.
        /// 3 - Warning - rule violations are reported to the user as warnings. Schemas are inserted in the group despite violations with Warning severity.
        /// 4 - Error - rule violations are reported to the user as errors. Schemas that trigger errors are not inserted in the group.
        /// </summary>
        public int Severity { get; set; }

        /// <summary>
        /// When true the severity is replaced with the one from the global configuration.
        /// </summary>
        public bool Inherit { get; set; }
    }
}
