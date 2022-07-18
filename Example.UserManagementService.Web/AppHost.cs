using Funq;
using ServiceStack;
using Example.UserManagementService.Common;

namespace Example.UserManagementService.Web
{
    public class AppHost : AppHostBase
    {
        private AppConfigurator _configurator;
        private readonly string _environment;
        private readonly ConfigurationManager _configManager;

        public AppHost(ConfigurationManager configManager, string environment) : base(ServiceConstants.ServiceName, typeof(AppConfigurator).Assembly)
        {
            if (configManager == null) throw new ArgumentNullException(nameof(configManager));
            if (string.IsNullOrEmpty(environment)) throw new ArgumentNullException(nameof(environment));

            _environment = environment;
            _configManager = configManager;
        }

        public override void Configure(Container container)
        {
            _configurator = new AppConfigurator(this, _configManager, _environment);
        }
    }
}
