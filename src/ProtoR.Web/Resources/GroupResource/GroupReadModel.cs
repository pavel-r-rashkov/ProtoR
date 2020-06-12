namespace ProtoR.Web.Resources.GroupResource
{
    using System;

    /// <summary>
    /// Group resource.
    /// </summary>
    public class GroupReadModel : GroupWriteModel, ICreationInfo
    {
        /// <summary>
        /// Group ID.
        /// </summary>
        public int Id { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public string CreatedBy { get; set; }
    }
}
