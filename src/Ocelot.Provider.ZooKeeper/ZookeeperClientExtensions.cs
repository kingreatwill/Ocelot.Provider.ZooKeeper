namespace Ocelot.Provider.ZooKeeper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Ocelot.Provider.ZooKeeper.Client;

    public static class ZookeeperClientExtensions
    {
        public static async Task<string> GetAsync(this ZookeeperClient client, string path)
        {
            if (await client.ExistsAsync(path))
            {
                return Encoding.UTF8.GetString((await client.GetDataAsync(path)).ToArray());
            }

            return null;
        }

        public static async Task<bool> SetAsync(this ZookeeperClient client, string path, string value)
        {
            if (await client.ExistsAsync(path))
            {
                var result = await client.SetDataAsync(path, Encoding.UTF8.GetBytes(value));
                return result != null;
            }

            await client.CreateRecursiveAsync(path, Encoding.UTF8.GetBytes(value));

            return true;
        }

        public static async Task<Dictionary<string, string>> GetRangeAsync(this ZookeeperClient client, string path)
        {
            var dic = new Dictionary<string, string>();
            var srvNames = await client.GetChildrenAsync(path);
            foreach (var srv in srvNames)
            {
                var srvIds = await client.GetChildrenAsync(path + srv);
                foreach (var srvId in srvIds)
                {
                    var idPath = $"{path}{srv}/{srvId}";
                    var srvData = await client.GetDataAsync(idPath);
                    if (srvData != null && srvData.Any())
                    {
                        dic.Add(idPath, Encoding.UTF8.GetString(srvData.ToArray()));
                    }
                }
            }

            return dic;
        }
    }
}