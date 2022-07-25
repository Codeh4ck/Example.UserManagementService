using ServiceStack;
using ServiceStack.Validation;
using Codelux.Logging;
using Codelux.ServiceStack;
using Codelux.ServiceStack.Plugins;
using Example.UserManagementService.Common;
using Example.UserManagementService.Dependencies;
using Microsoft.Extensions.Configuration;
using ServiceStack.Api.OpenApi;
using DebugLogger = Codelux.Logging.DebugLogger;

namespace Example.UserManagementService
{
    public class AppConfigurator : CoreAppConfiguratorBase
    {
        public AppConfigurator(ServiceStackHost appHost, string environment, ConfigurationManager configManager) 
            : base(ServiceConstants.ServiceName, appHost)
        {
            appHost.ConfigurePlugin<PredefinedRoutesFeature>(x => x.JsonApiRoute = null);

            appHost.Plugins.Add(new CorsFeature());
            appHost.Plugins.Add(new RouteFeature());
            appHost.Plugins.Add(new OpenApiFeature());
            appHost.Plugins.Add(new MetadataFeature());
            appHost.Plugins.Add(new ValidationFeature());
            appHost.Plugins.Add(new CoreServiceFeature());
            appHost.Plugins.Add(new OrmLiteMappingFeature());

            appHost.Container.Register<ConfigurationManager>(configManager);

            ILogger logger = new DebugLogger();
            appHost.Container.Register<ILogger>(logger);

            logger.LogEvent(LogType.Info, "Service is starting up.");

            new MappersModule(appHost).RegisterDependencies();
            new DatabaseModule(appHost).RegisterDependencies();
            new UtilitiesModule(appHost).RegisterDependencies();
            new ExecutorsModule(appHost).RegisterDependencies();

            appHost.Container.RegisterValidators(typeof(AppConfigurator).Assembly);
        }
    }
}