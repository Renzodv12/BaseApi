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

namespace Infra.Cache.Service
{
    public class CacheManager<T> : ICacheManager<T> where T : IEntity
    {
        IDatabase _redis;
        private const string _one_suffix = "id_";
        private const string _list_suffix = "list";
        string _key;
        string _one_key;
        string _list_key;
        CacheSettings _settings;
        EntitySetting _entitySettings;
        IConnectionMultiplexer _connection;
        public CacheManager(IConnectionMultiplexer connection,
                            IOptions<CacheSettings> cacheSettings)
        {

            _connection = connection;

            //creo un esquema genérico de claves para la caché
            _key = typeof(T).Name;
            _list_key = $"{_key}_{_list_suffix}";
            _one_key = $"{_key}_{_one_suffix}";

            _settings = cacheSettings.Value;
            _entitySettings = _settings.EntitySettings.Where(e => e.Name.Equals(_key)).FirstOrDefault();
            _redis = _connection.GetDatabase();
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
    }
}
