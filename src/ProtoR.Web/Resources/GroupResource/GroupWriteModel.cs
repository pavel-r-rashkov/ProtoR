namespace ProtoR.Web.Resources.GroupResource
{
    /// <summary>
    /// Group resource.
    /// </summary>
    public class GroupWriteModel
    {
        /// <summary>
        /// Group name.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// ID of the category this group belongs to.
        /// When not specified the group is assigned to default category.
        /// </summary>
        public long? CategoryId { get; set; }
    }
}
