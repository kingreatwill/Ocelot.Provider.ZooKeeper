namespace Ocelot.Provider.ZooKeeper
{
    using System;
    using dotnet_Zookeeper;

    public class ZookeeperClientFactory : IZookeeperClientFactory
    {
        public ZookeeperClient Get(ZookeeperRegistryConfiguration config)
        {
            return new ZookeeperClient(config.Host, config.Port);
        }
    }
}