using Funq;
using ServiceStack;
using Example.UserManagementService.Common;

namespace Example.UserManagementService.Web
{
    public class AppHost : AppHostBase
    {
        private AppConfigurator _configurator;
        private readonly string _environment;

        public AppHost(string environment) : base(ServiceConstants.ServiceName, typeof(AppConfigurator).Assembly)
        {
            if (string.IsNullOrEmpty(environment)) throw new ArgumentNullException(nameof(environment));
            _environment = environment;
        }

        public override void Configure(Container container)
        {
            _configurator = new AppConfigurator(this, _environment);
        }
    }
}
