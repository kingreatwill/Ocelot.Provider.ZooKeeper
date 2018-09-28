namespace Ocelot.Provider.ZooKeeper
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using ServiceDiscovery.Providers;
    using Values;

    public class PollZookeeper : IServiceDiscoveryProvider
    {
        private readonly IOcelotLogger _logger;
        private readonly IServiceDiscoveryProvider _ZookeeperServiceDiscoveryProvider;
        private readonly Timer _timer;
        private bool _polling;
        private List<Service> _services;

        public PollZookeeper(int pollingInterval, IOcelotLoggerFactory factory, IServiceDiscoveryProvider ZookeeperServiceDiscoveryProvider)
        {
            _logger = factory.CreateLogger<PollZookeeper>();
            _ZookeeperServiceDiscoveryProvider = ZookeeperServiceDiscoveryProvider;
            _services = new List<Service>();

            _timer = new Timer(
                async x =>
            {
                if (_polling)
                {
                    return;
                }

                _polling = true;
                await Poll();
                _polling = false;
            }, null, pollingInterval, pollingInterval);
        }

        public Task<List<Service>> Get()
        {
            return Task.FromResult(_services);
        }

        private async Task Poll()
        {
            _services = await _ZookeeperServiceDiscoveryProvider.Get();
        }
    }
}