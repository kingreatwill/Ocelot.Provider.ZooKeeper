namespace Ocelot.Provider.ZooKeeper
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Configuration.File;
    using Configuration.Repository;
    using Logging;
    using Newtonsoft.Json;
    using Ocelot.Provider.ZooKeeper.Client;
    using Responses;

    public class ZookeeperFileConfigurationRepository : IFileConfigurationRepository
    {
        private readonly ZookeeperClient _ZookeeperClient;
        private readonly string _configurationKey;
        private readonly Cache.IOcelotCache<FileConfiguration> _cache;
        private readonly IOcelotLogger _logger;

        public ZookeeperFileConfigurationRepository(
            Cache.IOcelotCache<FileConfiguration> cache,
            IInternalConfigurationRepository repo,
            IZookeeperClientFactory factory,
            IOcelotLoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ZookeeperFileConfigurationRepository>();
            _cache = cache;

            var internalConfig = repo.Get();

            _configurationKey = "InternalConfiguration";

            string token = null;

            if (!internalConfig.IsError)
            {
                token = internalConfig.Data.ServiceProviderConfiguration.Token;
                _configurationKey = !string.IsNullOrEmpty(internalConfig.Data.ServiceProviderConfiguration.ConfigurationKey) ?
                    internalConfig.Data.ServiceProviderConfiguration.ConfigurationKey : _configurationKey;
            }

            var config = new ZookeeperRegistryConfiguration(
                internalConfig.Data.ServiceProviderConfiguration.Host,
                internalConfig.Data.ServiceProviderConfiguration.Port, _configurationKey);

            _ZookeeperClient = factory.Get(config);
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            var config = _cache.Get(_configurationKey, _configurationKey);

            if (config != null)
            {
                return new OkResponse<FileConfiguration>(config);
            }
            var queryResult = await _ZookeeperClient.GetAsync($"/{_configurationKey}");
            if (string.IsNullOrWhiteSpace(queryResult))
            {
                return new OkResponse<FileConfiguration>(null);
            }
            var ZookeeperConfig = JsonConvert.DeserializeObject<FileConfiguration>(queryResult);
            return new OkResponse<FileConfiguration>(ZookeeperConfig);
        }

        public async Task<Response> Set(FileConfiguration ocelotConfiguration)
        {
            var json = JsonConvert.SerializeObject(ocelotConfiguration, Formatting.Indented);
            var result = await _ZookeeperClient.SetAsync($"/{_configurationKey}", json);

            _cache.AddAndDelete(_configurationKey, ocelotConfiguration, TimeSpan.FromSeconds(3), _configurationKey);

            return new OkResponse();

            // return new ErrorResponse(new UnableToSetConfigInZookeeperError($"Unable to set FileConfiguration in Zookeeper, response status code from Zookeeper was {result.StatusCode}"));
        }
    }
}