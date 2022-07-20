using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Codelux.Configuration;
using Codelux.ServiceStack.Utilities;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Dependencies
{
    public class DatabaseModule : DependencyModuleBase
    {
        public DatabaseModule(ServiceStackHost appHost) : base(appHost) { }

        public override void RegisterDependencies()
        {
            IConfigSource configSource = AppHost.Container.Resolve<IConfigSource>();
            configSource.TryGetString("dbconnectionstring", out string connectionString);

            SqlServerDialect.Provider.GetStringConverter().UseUnicode = true;
            IDbConnectionFactory dbConnectionFactory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);

            AppHost.Register<IDbConnectionFactory>(dbConnectionFactory);

            AppHost.Container.RegisterAutoWiredAs<UserRepository, IUserRepository>();
        }
    }
}
