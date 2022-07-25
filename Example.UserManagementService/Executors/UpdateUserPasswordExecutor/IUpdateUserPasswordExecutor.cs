using Codelux.Executors;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;

namespace Example.UserManagementService.Executors.UpdateUserPasswordExecutor
{
    public interface IUpdateUserPasswordExecutor : IExecutor<UpdateUserPasswordRequest, UpdateUserPasswordResponse> { }
}
