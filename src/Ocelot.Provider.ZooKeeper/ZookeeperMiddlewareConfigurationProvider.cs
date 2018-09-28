namespace Ocelot.Provider.ZooKeeper
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration.Creator;
    using Configuration.File;
    using Configuration.Repository;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Middleware;
    using Responses;

    public static class ZookeeperMiddlewareConfigurationProvider
    {
        public static OcelotMiddlewareConfigurationDelegate Get = async builder =>
        {
            var fileConfigRepo = builder.ApplicationServices.GetService<IFileConfigurationRepository>();
            var fileConfig = builder.ApplicationServices.GetService<IOptionsMonitor<FileConfiguration>>();
            var internalConfigCreator = builder.ApplicationServices.GetService<IInternalConfigurationCreator>();
            var internalConfigRepo = builder.ApplicationServices.GetService<IInternalConfigurationRepository>();

            if (UsingZookeeper(fileConfigRepo))
            {
                await SetFileConfigInZookeeper(builder, fileConfigRepo, fileConfig, internalConfigCreator, internalConfigRepo);
            }
        };

        private static bool UsingZookeeper(IFileConfigurationRepository fileConfigRepo)
        {
            return fileConfigRepo.GetType() == typeof(ZookeeperFileConfigurationRepository);
        }

        private static async Task SetFileConfigInZookeeper(IApplicationBuilder builder,
            IFileConfigurationRepository fileConfigRepo, IOptionsMonitor<FileConfiguration> fileConfig,
            IInternalConfigurationCreator internalConfigCreator, IInternalConfigurationRepository internalConfigRepo)
        {
            // get the config from Zookeeper.
            var fileConfigFromZookeeper = await fileConfigRepo.Get();

            if (IsError(fileConfigFromZookeeper))
            {
                ThrowToStopOcelotStarting(fileConfigFromZookeeper);
            }
            else if (ConfigNotStoredInZookeeper(fileConfigFromZookeeper))
            {
                //there was no config in Zookeeper set the file in config in Zookeeper
                await fileConfigRepo.Set(fileConfig.CurrentValue);
            }
            else
            {
                // create the internal config from Zookeeper data
                var internalConfig = await internalConfigCreator.Create(fileConfigFromZookeeper.Data);

                if (IsError(internalConfig))
                {
                    ThrowToStopOcelotStarting(internalConfig);
                }
                else
                {
                    // add the internal config to the internal repo
                    var response = internalConfigRepo.AddOrReplace(internalConfig.Data);

                    if (IsError(response))
                    {
                        ThrowToStopOcelotStarting(response);
                    }
                }

                if (IsError(internalConfig))
                {
                    ThrowToStopOcelotStarting(internalConfig);
                }
            }
        }

        private static void ThrowToStopOcelotStarting(Response config)
        {
            throw new Exception($"Unable to start Ocelot, errors are: {string.Join(",", config.Errors.Select(x => x.ToString()))}");
        }

        private static bool IsError(Response response)
        {
            return response == null || response.IsError;
        }

        private static bool ConfigNotStoredInZookeeper(Response<FileConfiguration> fileConfigFromZookeeper)
        {
            return fileConfigFromZookeeper.Data == null;
        }
    }
}