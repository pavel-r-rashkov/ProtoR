namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using Apache.Ignite.Core;
    using Microsoft.Extensions.Options;

    public class BaseDataProvider
    {
        public BaseDataProvider(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider)
        {
            _ = igniteFactory ?? throw new ArgumentNullException(nameof(igniteFactory));
            _ = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));

            this.Ignite = igniteFactory.Instance();
            this.ConfigurationProvider = configurationProvider.Value;
        }

        protected IIgnite Ignite { get; }

        protected IgniteExternalConfiguration ConfigurationProvider { get; }
    }
}
