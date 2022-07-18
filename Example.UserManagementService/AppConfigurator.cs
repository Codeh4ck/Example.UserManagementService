using Codelux.ServiceStack;
using Example.UserManagementService.Common;
using Microsoft.Extensions.Configuration;
using ServiceStack;

namespace Example.UserManagementService
{
    public class AppConfigurator : CoreAppConfiguratorBase
    {
        public AppConfigurator(ServiceStackHost appHost, ConfigurationManager configManager, string environment) 
            : base(ServiceConstants.ServiceName, appHost)
        {

        }
    }
}