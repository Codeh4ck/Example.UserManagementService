using Codelux.Executors;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;

namespace Example.UserManagementService.Executors.AuthenticateUserExecutor
{
    public interface IAuthenticateUserExecutor : IExecutor<AuthenticateUserRequest, AuthenticateUserResponse> { }
}
