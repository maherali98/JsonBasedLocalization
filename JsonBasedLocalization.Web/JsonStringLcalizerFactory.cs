using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace JsonBasedLocalization.Web
{
    public class JsonStringLcalizerFactory : IStringLocalizerFactory
    { 
        private readonly IDistributedCache _cache;
    public JsonStringLcalizerFactory(IDistributedCache cache)
    {
        _cache= cache;
    }
        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_cache);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_cache);

        }
    }
}
