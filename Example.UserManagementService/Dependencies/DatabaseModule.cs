using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Codelux.ServiceStack.Utilities;
using Microsoft.Extensions.Configuration;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Dependencies
{
    public class DatabaseModule : DependencyModuleBase
    {
        public DatabaseModule(ServiceStackHost appHost) : base(appHost) { }

        public override void RegisterDependencies()
        {
            ConfigurationManager configManager = AppHost.Container.Resolve<ConfigurationManager>();
            string connectionString = configManager["dbconnectionstring"];

            SqlServerDialect.Provider.GetStringConverter().UseUnicode = true;
            IDbConnectionFactory dbConnectionFactory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);

            AppHost.Register<IDbConnectionFactory>(dbConnectionFactory);

            AppHost.Container.RegisterAutoWiredAs<UserRepository, IUserRepository>();
        }
    }
}
