namespace ProtoR.Infrastructure.DataAccess.DependencyInjection
{
    using Apache.Ignite.Core.Plugin;

    public class AutoFacPluginProvider : IPluginProvider<AutoFacPluginConfiguration>
    {
        private readonly AutoFacPlugin plugin;

        public AutoFacPluginProvider()
        {
            this.plugin = new AutoFacPlugin();
        }

        public string Name => nameof(AutoFacPluginProvider);

        public string Copyright => string.Empty;

        public T GetPlugin<T>()
            where T : class
        {
            return this.plugin as T;
        }

        public void OnIgniteStart()
        {
            // No-op
        }

        public void OnIgniteStop(bool cancel)
        {
            // No-op
        }

        public void Start(IPluginContext<AutoFacPluginConfiguration> context)
        {
            // No-op
        }

        public void Stop(bool cancel)
        {
            // No-op
        }
    }
}
