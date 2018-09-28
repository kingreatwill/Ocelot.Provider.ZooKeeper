namespace Ocelot.Provider.ZooKeeper
{
    using Errors;

    public class UnableToSetConfigInZookeeperError : Error
    {
        public UnableToSetConfigInZookeeperError(string s)
            : base(s, OcelotErrorCode.UnknownError)
        {
        }
    }
}