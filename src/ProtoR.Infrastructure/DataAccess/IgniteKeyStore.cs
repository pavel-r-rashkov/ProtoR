namespace ProtoR.Infrastructure.DataAccess
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Apache.Ignite.Linq;
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using Microsoft.Extensions.Options;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class IgniteKeyStore : IXmlRepository
    {
        private readonly IIgniteFactory igniteFactory;
        private IOptions<IgniteExternalConfiguration> igniteConfiguration;

        public IgniteKeyStore(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> igniteConfiguration)
        {
            this.igniteFactory = igniteFactory;
            this.igniteConfiguration = igniteConfiguration;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var ignite = this.igniteFactory.Instance();
            var keyCache = ignite.GetCache<long, KeyCacheItem>(this.igniteConfiguration.Value.CacheNames.KeyCacheName);

            return keyCache
                .AsCacheQueryable()
                .ToList()
                .Select(k => XElement.Parse(k.Value.XmlElement))
                .ToList();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var ignite = this.igniteFactory.Instance();
            var keyCache = ignite.GetCache<long, KeyCacheItem>(this.igniteConfiguration.Value.CacheNames.KeyCacheName);

            var keyIdGenerator = ignite.GetAtomicSequence(
                $"{typeof(KeyCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);

            keyCache.Put(keyIdGenerator.Increment(), new KeyCacheItem
            {
                XmlElement = element.ToString(),
                FriendlyName = friendlyName,
            });
        }
    }
}
