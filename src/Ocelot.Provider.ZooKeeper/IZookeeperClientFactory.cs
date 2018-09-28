using Ocelot.Provider.ZooKeeper.Client;

namespace Ocelot.Provider.ZooKeeper
{
    public interface IZookeeperClientFactory
    {
        ZookeeperClient Get(ZookeeperRegistryConfiguration config);
    }
}