namespace Ocelot.Provider.ZooKeeper
{
    public class ZookeeperRegistryConfiguration
    {
        public ZookeeperRegistryConfiguration(string host, int port, string keyOfServiceInZookeeper)
        {
            this.Host = string.IsNullOrEmpty(host) ? "localhost" : host;
            this.Port = port > 0 ? port : 2181;
            this.KeyOfServiceInZookeeper = keyOfServiceInZookeeper;

            // this.Token = token;
        }

        public string KeyOfServiceInZookeeper { get; }

        public string Host { get; }

        public int Port { get; }

        // public string Token { get; }
    }
}