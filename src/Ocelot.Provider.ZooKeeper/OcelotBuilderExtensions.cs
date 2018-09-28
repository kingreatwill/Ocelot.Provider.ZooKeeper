namespace Ocelot.Provider.ZooKeeper
{
    using Configuration.Repository;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware;
    using ServiceDiscovery;

    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddZookeeper(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton<ServiceDiscoveryFinderDelegate>(ZookeeperProviderFactory.Get);
            builder.Services.AddSingleton<IZookeeperClientFactory, ZookeeperClientFactory>();
            return builder;
        }

        public static IOcelotBuilder AddConfigStoredInZookeeper(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton<OcelotMiddlewareConfigurationDelegate>(ZookeeperMiddlewareConfigurationProvider.Get);
            builder.Services.AddHostedService<FileConfigurationPoller>();
            builder.Services.AddSingleton<IFileConfigurationRepository, ZookeeperFileConfigurationRepository>();
            return builder;
        }
    }
}