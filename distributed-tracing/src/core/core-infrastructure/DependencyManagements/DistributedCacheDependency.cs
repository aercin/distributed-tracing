using core_application.Abstractions;
using core_infrastructure.Services;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;

namespace core_infrastructure.DependencyManagements
{
    public static class DistributedCacheDependency
    {
        public static IServiceCollection AddDistributedCaching(this IServiceCollection services, Action<Options> options)
        {
            var dependencyOptions = new Options();
            options(dependencyOptions);

            services.AddSingleton<ICacheProvider, CacheProvider>();

            var configOption = new ConfigurationOptions
            {
                EndPoints =
                    {
                         { dependencyOptions.RedisHost,dependencyOptions.RedisPort }
                    },
                Password = dependencyOptions.RedisPassword,
                DefaultDatabase = dependencyOptions.RedisDefaultDb
            };

            services.AddStackExchangeRedisCache(config =>
            {
                config.Configuration = configOption.ToString();
            }); 

            return services;
        }

        public sealed class Options
        { 
            public string RedisHost { get; set; }
            public int RedisPort { get; set; }
            public string RedisPassword { get; set; }
            public int RedisDefaultDb { get; set; }
        }

        public static ConnectionMultiplexer GetConnection(this RedisCache cache)
        {
            //ensure connection is established
            typeof(RedisCache).InvokeMember("Connect", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, cache, new object[] { });

            //get connection multiplexer
            var fi = typeof(RedisCache).GetField("_connection", BindingFlags.Instance | BindingFlags.NonPublic);
            var connection = (ConnectionMultiplexer)fi.GetValue(cache);
            return connection;
        } 
    }
}
