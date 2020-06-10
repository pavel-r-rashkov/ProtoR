namespace ProtoR.Infrastructure.DataAccess.Repositories
{
    using System;
    using Apache.Ignite.Core;
    using Microsoft.Extensions.Options;
    using ProtoR.Application;

    public class BaseRepository
    {
        public BaseRepository(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider,
            IUserProvider userProvider)
        {
            _ = igniteFactory ?? throw new ArgumentNullException(nameof(igniteFactory));
            _ = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _ = userProvider ?? throw new ArgumentNullException(nameof(userProvider));

            this.Ignite = igniteFactory.Instance();
            this.ConfigurationProvider = configurationProvider.Value;
            this.UserProvider = userProvider;
        }

        protected IIgnite Ignite { get; }

        protected IgniteExternalConfiguration ConfigurationProvider { get; }

        protected IUserProvider UserProvider { get; }
    }
}
