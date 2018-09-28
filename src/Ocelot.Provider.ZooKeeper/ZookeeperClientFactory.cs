using Ocelot.Provider.ZooKeeper.Client;

namespace Ocelot.Provider.ZooKeeper
{
    public class ZookeeperClientFactory : IZookeeperClientFactory
    {
        public ZookeeperClient Get(ZookeeperRegistryConfiguration config)
        {
            return new ZookeeperClient($"{config.Host}:{config.Port}");
        }
    }
}