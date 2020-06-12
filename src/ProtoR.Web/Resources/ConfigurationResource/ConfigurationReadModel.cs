namespace ProtoR.Web.Resources.ConfigurationResource
{
    /// <summary>
    /// Configuration resource.
    /// </summary>
    public class ConfigurationReadModel : ConfigurationWriteModel
    {
        /// <summary>
        /// Configuration ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the group this configuration belongs to or null if this is global configuration.
        /// </summary>
        public int? GroupId { get; set; }
    }
}
