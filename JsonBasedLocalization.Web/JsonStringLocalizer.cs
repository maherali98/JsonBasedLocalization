using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JsonBasedLocalization.Web
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _jsonSerializer = new();
        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache= cache;
        }
        public LocalizedString this[string name]
        {
            get 
            {
                var Value = GetString(name);
                return new LocalizedString(name, Value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var ActualValue = this[name];
                return !ActualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(ActualValue.Value,arguments))
                     : ActualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var FilePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            using FileStream stream = new(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamreader = new(stream);
            using JsonTextReader Reader = new(streamreader);
            while (Reader.Read())
            {
                if (Reader.TokenType != JsonToken.PropertyName)
                    continue;
                var Key = Reader.Value as string;
                Reader.Read();
                var value = _jsonSerializer.Deserialize<string>(Reader);
               yield return new LocalizedString(Key, value);
            }
        }
        private string GetString (string Key)
        {
            //Resources/ar-EG.json
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var FullFilePath = Path.GetFullPath(filePath);
            if (File.Exists(FullFilePath))
            {
                var CashKey = $"local_{Thread.CurrentThread.CurrentCulture.Name}_{Key}";
                var casheValue = _cache.GetString(CashKey);
                if (!string.IsNullOrEmpty(casheValue))
                    return casheValue;

                var result = GetValueFromJson(Key , FullFilePath) ;
                if (!string.IsNullOrEmpty(result))
                    _cache.SetString(CashKey, result);
                return result ;
            }
            return string.Empty;
        }
        private string GetValueFromJson(string PropertyName , string FilePath)
        {
            if (string.IsNullOrEmpty(PropertyName) || string.IsNullOrEmpty(FilePath))
                return string.Empty;
            using FileStream stream = new(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamreader = new(stream);
            using JsonTextReader Reader = new(streamreader);

            while (Reader.Read())
            {
                if (Reader.TokenType == JsonToken.PropertyName && Reader.Value as string == PropertyName)
                {
                    Reader.Read();
                    return _jsonSerializer.Deserialize<string>(Reader);
                }
            }
            return string.Empty;
        }

    }
}
