namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    /// <summary>
    /// Rule codes.
    /// </summary>
    public enum RuleCode
    {
        /// <summary>
        /// Code for test rule.
        /// </summary>
        R0001,

        /// <summary>
        /// Message type is removed.
        /// </summary>
        PB0001,

        /// <summary>
        /// Message type is added.
        /// </summary>
        PB0002,

        /// <summary>
        /// Enum type is removed.
        /// </summary>
        PB0003,

        /// <summary>
        /// Enum type is added.
        /// </summary>
        PB0004,

        /// <summary>
        /// OneOf type is removed.
        /// </summary>
        PB0005,

        /// <summary>
        /// OneOf type is added.
        /// </summary>
        PB0006,

        /// <summary>
        /// Field is removed.
        /// </summary>
        PB0007,

        /// <summary>
        /// Field is added.
        /// </summary>
        PB0008,
    }
}
