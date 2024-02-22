using Newtonsoft.Json;

using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
namespace ServerProject.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IConnectionMultiplexer muxer, ILogger<CacheService> logger)
        {
            _db = muxer.GetDatabase();
            _logger = logger;
        }
        public T GetData<T>(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                var dataCahed = JsonConvert.DeserializeObject<T>(value!);
                _logger.LogInformation("Data get from cache" + dataCahed);
                return dataCahed;

            }

            return default;
        }
        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

            Console.WriteLine(value);

            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            _logger.LogInformation("Data has cached !!!" + value.ToString());
            return isSet;
        }
        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }
        public object RemoveDataWithPrefix(string prefix)
        {
            var _isKeyExist = _db.WithKeyPrefix(prefix);
            if (_isKeyExist != null)
            {
                return _db.KeyDelete(prefix);
            }
            return false;
        }
    }
}
