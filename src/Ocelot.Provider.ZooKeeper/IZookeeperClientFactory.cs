namespace Ocelot.Provider.ZooKeeper
{
    using dotnet_Zookeeper;

    public interface IZookeeperClientFactory
    {
        ZookeeperClient Get(ZookeeperRegistryConfiguration config);
    }
}