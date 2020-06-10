namespace ProtoR.Infrastructure.DataAccess.Repositories
{
    using System;
    using Apache.Ignite.Core;
    using ProtoR.Application;

    public class BaseRepository
    {
        public BaseRepository(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider,
            IUserProvider userProvider)
        {
            _ = igniteFactory ?? throw new ArgumentNullException(nameof(igniteFactory));
            _ = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
            _ = userProvider ?? throw new ArgumentNullException(nameof(userProvider));

            this.Ignite = igniteFactory.Instance();
            this.ConfigurationProvider = configurationProvider;
            this.UserProvider = userProvider;
        }

        protected IIgnite Ignite { get; }

        protected IIgniteConfiguration ConfigurationProvider { get; }

        protected IUserProvider UserProvider { get; }
    }
}
