namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using Apache.Ignite.Core;

    public class BaseDataProvider
    {
        public BaseDataProvider(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider)
        {
            _ = igniteFactory ?? throw new ArgumentNullException(nameof(igniteFactory));
            _ = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));

            this.Ignite = igniteFactory.Instance();
            this.ConfigurationProvider = configurationProvider;
        }

        protected IIgnite Ignite { get; }

        protected IIgniteConfiguration ConfigurationProvider { get; }
    }
}
