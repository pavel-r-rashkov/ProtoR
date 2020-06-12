namespace ProtoR.Web.Resources.SchemaResource
{
    using System;

    /// <summary>
    /// Schema resource.
    /// </summary>
    public class SchemaReadModel : ICreationInfo
    {
        /// <summary>
        /// Schema ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Schema contents.
        /// </summary>
        public string Contents { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public string CreatedBy { get; set; }
    }
}
