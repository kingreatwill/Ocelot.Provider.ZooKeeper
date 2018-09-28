using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ocelot.Provider.ZooKeeper
{
    public class ZookeeperClient : Watcher
    {
        ////"localhost:2181"
        public ZookeeperClient(string address)
        {
        }

        public Task<string> GetAsync(string path)
        {
            return Task.FromResult<string>(string.Empty);
        }

        public Task<bool> SetAsync(string path, string value)
        {
            return Task.FromResult(true);
        }

        public Task<Dictionary<string, string>> GetRangeAsync(string path)
        {
            return Task.FromResult(new Dictionary<string, string>());
        }

        public override Task process(WatchedEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}