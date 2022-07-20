using ServiceStack;
using ServiceStack.Validation;
using Codelux.Logging;
using Codelux.ServiceStack;
using Codelux.ServiceStack.Plugins;
using Example.UserManagementService.Common;
using Example.UserManagementService.Dependencies;
using DebugLogger = Codelux.Logging.DebugLogger;

namespace Example.UserManagementService
{
    public class AppConfigurator : CoreAppConfiguratorBase
    {
        public AppConfigurator(ServiceStackHost appHost, string environment) : base(ServiceConstants.ServiceName, appHost)
        {
            appHost.Plugins.Add(new ValidationFeature());
            appHost.Plugins.Add(new MetadataFeature());
            appHost.Plugins.Add(new OrmLiteMappingFeature());
            appHost.Plugins.Add(new CoreServiceFeature());
            appHost.Plugins.Add(new CorsFeature());
            appHost.Plugins.Add(new RouteFeature());

            ILogger logger = new DebugLogger();
            appHost.Container.Register<ILogger>(logger);

            logger.LogEvent(LogType.Info, "Service is starting up.");

            new UtilitiesModule(appHost).RegisterDependencies();
            new DatabaseModule(appHost).RegisterDependencies();

            appHost.Container.RegisterValidators(typeof(AppConfigurator).Assembly);
        }
    }
}