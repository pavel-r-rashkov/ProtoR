namespace ProtoR.Infrastructure.DataAccess.DependencyInjection
{
    using Apache.Ignite.Core.Binary;
    using Apache.Ignite.Core.Plugin;

    [PluginProviderType(typeof(AutoFacPluginProvider))]
    public class AutoFacPluginConfiguration : IPluginConfiguration
    {
        public int? PluginConfigurationClosureFactoryId => null;

        public void WriteBinary(IBinaryRawWriter writer)
        {
            // No-op
        }
    }
}
