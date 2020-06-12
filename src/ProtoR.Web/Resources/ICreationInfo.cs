namespace ProtoR.Web.Resources
{
    using System;

    public interface ICreationInfo
    {
        /// <summary>
        /// Creation date in UTC.
        /// </summary>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// Name of the user or client who created this resource.
        /// </summary>
        string CreatedBy { get; set; }
    }
}
