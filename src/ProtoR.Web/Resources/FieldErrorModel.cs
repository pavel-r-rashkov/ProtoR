namespace ProtoR.Web.Resources
{
    using System.Collections.Generic;

    /// <summary>
    /// Field error.
    /// </summary>
    public class FieldErrorModel
    {
        /// <summary>
        /// Field name.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Error messages.
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; set; }
    }
}
