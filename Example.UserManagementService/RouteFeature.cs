using ServiceStack;
using Example.UserManagementService.Common.Requests;

namespace Example.UserManagementService
{
    public class RouteFeature : IPlugin
    {
        public void Register(IAppHost appHost)
        {
            appHost.Routes.Add<RegisterUserRequest>("/api/users", ApplyTo.Post);
            appHost.Routes.Add<AuthenticateUserRequest>("/api/users", ApplyTo.Get);
            appHost.Routes.Add<UpdateUserEmailRequest>("/api/users/email", ApplyTo.Put);
            appHost.Routes.Add<UpdateUserPasswordRequest>("/api/users/password", ApplyTo.Put);
        }
    }
}
