using Codelux.Executors;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;

namespace Example.UserManagementService.Executors.UpdateUserEmailExecutor
{
    public interface IUpdateUserEmailExecutor : IExecutor<UpdateUserEmailRequest, UpdateUserEmailResponse> { }
}
