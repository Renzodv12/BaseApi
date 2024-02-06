using System;
using Core.Interfaces;
using StackExchange.Redis;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using Infra.Cache.Settings;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using Microsoft.Extensions.Primitives;

namespace Infra.Cache.Service
{
    public class CacheManager<T> : ICacheManager<T> where T : IEntity
    {
        IDatabase _redis;
        private IMemoryCache _memoryCache;
        private const string _one_suffix = "id_";
        private const string _list_suffix = "list";
        string _key;
        string _one_key;
        string _list_key;
        CacheSettings _settings;
        EntitySetting _entitySettings;
        IConnectionMultiplexer _connection;
        private static CancellationTokenSource _resetCacheToken = new();
        protected readonly ConcurrentDictionary<string, SemaphoreSlim> CacheEntries = new();
        public CacheManager(IConnectionMultiplexer connection,
                            IOptions<CacheSettings> cacheSettings, IMemoryCache memoryCache)
        {

            _connection = connection;

            //creo un esquema genérico de claves para la caché
            _key = typeof(T).Name;
            _list_key = $"{_key}_{_list_suffix}";
            _one_key = $"{_key}_{_one_suffix}";

            _settings = cacheSettings.Value;
            _entitySettings = _settings.EntitySettings.Where(e => e.Name.Equals(_key)).FirstOrDefault();
            _redis = _connection.GetDatabase();
            _memoryCache = memoryCache;
        }

        private TimeSpan GetTimespan()
        {

            if (_entitySettings == null) throw new InvalidOperationException($"Cannot set Expiry from {_key}.");

            switch (_entitySettings.Period)
            {
                case Period.s:
                    return TimeSpan.FromSeconds(_entitySettings.Expiry);
                case Period.m:
                    return TimeSpan.FromMinutes(_entitySettings.Expiry);
                case Period.h:
                    return TimeSpan.FromHours(_entitySettings.Expiry);
                case Period.d:
                    return TimeSpan.FromDays(_entitySettings.Expiry);
                default:
                    return TimeSpan.Zero;
            }
        }

        public async Task<IList<T>> GetList()
        {

            var data = await _redis.StringGetAsync(_list_key);

            if (!data.HasValue) return default(IList<T>);

            var redisData = Encoding.UTF8.GetString(data);
            var entity = JsonConvert.DeserializeObject<IList<T>>(redisData);
            return entity;
        }

        public async Task<T> GetOne(int id)
        {
            var data = await _redis.StringGetAsync($"{_one_key}{id}");

            if (!data.HasValue) return default(T);

            var redisData = Encoding.UTF8.GetString(data);
            var entity = JsonConvert.DeserializeObject<T>(redisData);

            return entity;
        }


        public async void SetList(IList<T> entity)
        {
            var data = JsonConvert.SerializeObject(entity);
            var redisData = Encoding.UTF8.GetBytes(data);

            var ts = GetTimespan();
            if (ts == TimeSpan.Zero)
                await _redis.StringSetAsync(_list_key, redisData);
            else
                await _redis.StringSetAsync(_list_key, redisData, ts);

        }

        public async void SetOne(T entity)
        {
            var data = JsonConvert.SerializeObject(entity);
            var redisData = Encoding.UTF8.GetBytes(data);

            var ts = GetTimespan();
            if (ts == TimeSpan.Zero)
                await _redis.StringSetAsync($"{_one_key}{entity.Id}", redisData);
            else
                await _redis.StringSetAsync($"{_one_key}{entity.Id}", redisData, ts);


        }

        public bool IsCacheableEntity()
        {
            return _entitySettings != null;
        }
        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(int cacheTime)
        {
            var options = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTime) }
            .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token))
            .RegisterPostEvictionCallback(PostEvictionCallback);
            return options;
        }
        private void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason != EvictionReason.Replaced)
                CacheEntries.TryRemove(key?.ToString() ?? "", out var _);
        }
        public virtual Task<T> GetAsync<T>(string key, Func<Task<T>> acquire)
        {
            return GetAsync(key, acquire, _entitySettings.Expiry);
        }
        public virtual async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, long cacheTime)
        {
            if (_memoryCache.TryGetValue(key, out T cacheEntry)) return cacheEntry;
            SemaphoreSlim semaphore = CacheEntries.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            try
            {
                if (!_memoryCache.TryGetValue(key, out T cacheEntry))
                {
                    cacheEntry = await acquire();
                    _memoryCache.Set(key, cacheEntry, GetMemoryCacheEntryOptions(cacheTime));
                }
            }
            finally
            {
                semaphore.Release();
            }
            return cacheEntry;

        }
        public virtual Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
