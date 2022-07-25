using ServiceStack;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Executors.AuthenticateUserExecutor;
using Example.UserManagementService.Executors.RegisterUserExecutor;
using Example.UserManagementService.Executors.UpdateUserEmailExecutor;
using Example.UserManagementService.Executors.UpdateUserPasswordExecutor;

namespace Example.UserManagementService
{
    public class UserService : Service
    {
        private readonly IRegisterUserExecutor _registerUserExecutor;
        private readonly IAuthenticateUserExecutor _authenticateUserExecutor;
        private readonly IUpdateUserEmailExecutor _updateUserEmailExecutor;
        private readonly IUpdateUserPasswordExecutor _updateUserPasswordExecutor;

        public UserService(
            IRegisterUserExecutor registerUserExecutor,
            IAuthenticateUserExecutor authenticateUserExecutor,
            IUpdateUserEmailExecutor updateUserEmailExecutor,
            IUpdateUserPasswordExecutor updateUserPasswordExecutor)
        {
            registerUserExecutor.Guard(nameof(registerUserExecutor));
            authenticateUserExecutor.Guard(nameof(authenticateUserExecutor));
            updateUserEmailExecutor.Guard(nameof(updateUserEmailExecutor));
            updateUserPasswordExecutor.Guard(nameof(updateUserPasswordExecutor));

            _registerUserExecutor = registerUserExecutor;
            _authenticateUserExecutor = authenticateUserExecutor;
            _updateUserEmailExecutor = updateUserEmailExecutor;
            _updateUserPasswordExecutor = updateUserPasswordExecutor;
        }

        public Task<RegisterUserResponse> Post(RegisterUserRequest request) => _registerUserExecutor.ExecuteAsync(request);
        public Task<AuthenticateUserResponse> Get(AuthenticateUserRequest request) => _authenticateUserExecutor.ExecuteAsync(request);
        public Task<UpdateUserEmailResponse> Put(UpdateUserEmailRequest request) => _updateUserEmailExecutor.ExecuteAsync(request);
        public Task<UpdateUserPasswordResponse> Put(UpdateUserPasswordRequest request) => _updateUserPasswordExecutor.ExecuteAsync(request);
    }
}
