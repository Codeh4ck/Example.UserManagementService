using Codelux.Executors;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;

namespace Example.UserManagementService.Executors.RegisterUserExecutor
{
    public interface IRegisterUserExecutor : IExecutor<RegisterUserRequest, RegisterUserResponse> { }
}
