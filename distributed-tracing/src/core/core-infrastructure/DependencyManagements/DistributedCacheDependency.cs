using core_application.Abstractions;
using core_infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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
    }
}
