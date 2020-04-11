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

        /// <summary>
        /// Enum const is removed.
        /// </summary>
        PB0009,

        /// <summary>
        /// Enum const is added.
        /// </summary>
        PB0010,

        /// <summary>
        /// Field is renamed.
        /// </summary>
        PB0013,

        /// <summary>
        /// Enum const is renamed.
        /// </summary>
        PB0014,

        /// <summary>
        /// Removed field is missing a reservation.
        /// </summary>
        PB0015,

        /// <summary>
        /// OneOf field is removed.
        /// </summary>
        PB0016,
    }
}
