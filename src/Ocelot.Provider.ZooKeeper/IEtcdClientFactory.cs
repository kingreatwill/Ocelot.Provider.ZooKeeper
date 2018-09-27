namespace Ocelot.Provider.ZooKeeper
{
    using dotnet_etcd;

    public interface IEtcdClientFactory
    {
        EtcdClient Get(EtcdRegistryConfiguration config);
    }
}