using ServiceStack;
using Codelux.ServiceStack.Utilities;
using Example.UserManagementService.Executors.RegisterUserExecutor;
using Example.UserManagementService.Executors.AuthenticateUserExecutor;
using Example.UserManagementService.Executors.UpdateUserEmailExecutor;
using Example.UserManagementService.Executors.UpdateUserPasswordExecutor;

namespace Example.UserManagementService.Dependencies
{
    public class ExecutorsModule : DependencyModuleBase
    {
        public ExecutorsModule(ServiceStackHost appHost) : base(appHost) { }

        public override void RegisterDependencies()
        {
            AppHost.Container.RegisterAutoWiredAs<RegisterUserExecutor, IRegisterUserExecutor>();
            AppHost.Container.RegisterAutoWiredAs<AuthenticateUserExecutor, IAuthenticateUserExecutor>();
            AppHost.Container.RegisterAutoWiredAs<UpdateUserEmailExecutor, IUpdateUserEmailExecutor>();
            AppHost.Container.RegisterAutoWiredAs<UpdateUserPasswordExecutor, IUpdateUserPasswordExecutor>();
        }
    }
}
