namespace ProtoR.Web.Resources
{
    using System.Collections.Generic;

    /// <summary>
    /// Error response.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Field errors.
        /// </summary>
        public IEnumerable<FieldErrorModel> Errors { get; set; }
    }
}
