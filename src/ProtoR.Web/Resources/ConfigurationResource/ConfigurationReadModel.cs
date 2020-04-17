namespace ProtoR.Web.Resources.ConfigurationResource
{
    public class ConfigurationReadModel : ConfigurationWriteModel
    {
        public int Id { get; set; }

        public int? GroupId { get; set; }
    }
}
