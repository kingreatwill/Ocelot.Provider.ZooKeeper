namespace Ocelot.Provider.ZooKeeper
{
    using System;
    using dotnet_etcd;

    public class EtcdClientFactory : IEtcdClientFactory
    {
        public EtcdClient Get(EtcdRegistryConfiguration config)
        {
            return new EtcdClient(config.Host, config.Port);
        }
    }
}