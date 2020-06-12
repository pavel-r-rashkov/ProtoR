namespace ProtoR.Web.Resources.CategoryResource
{
    using System;

    /// <inheritdoc cref="CategoryWriteModel" />
    public class CategoryReadModel : CategoryWriteModel, ICreationInfo
    {
        /// <inheritdoc cref="ICreationInfo" />
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public string CreatedBy { get; set; }
    }
}
