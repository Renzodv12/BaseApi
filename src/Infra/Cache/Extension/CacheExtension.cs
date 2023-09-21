using System;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Core.Interfaces;
using Infra.Cache.Settings;
using Infra.Cache.Service;
using Microsoft.Extensions.Hosting;

namespace Infra.Cache.Extension
{
	public static class CacheExtension
	{
        public static IServiceCollection UseRedisCache(this IServiceCollection service, IConfiguration config)
        {
            if (!config.GetSection("Endpoint").Exists())
            {
                throw new ArgumentNullException("Config no existe");
            }

            service.Configure<CacheSettings>(options => config.Bind(options));
            ConnectionMultiplexer cm = ConnectionMultiplexer.Connect(config.GetSection("Endpoint").Value);
            service.AddSingleton<IConnectionMultiplexer>(cm);
            service.AddScoped(typeof(ICacheManager<>), typeof(CacheManager<>));


            return service;
        }
    }
}

