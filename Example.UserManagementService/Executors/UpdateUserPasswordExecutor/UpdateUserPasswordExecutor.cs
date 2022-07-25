using Codelux.Executors;
using Codelux.Utilities;
using Codelux.Utilities.Crypto;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Executors.UpdateUserPasswordExecutor
{
    public class UpdateUserPasswordExecutor : ExecutorBase<UpdateUserPasswordRequest, UpdateUserPasswordResponse>, IUpdateUserPasswordExecutor
    {
        private readonly IClockService _clockService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncryptor _passwordEncryptor;

        public UpdateUserPasswordExecutor(IClockService clockService, IUserRepository userRepository, IPasswordEncryptor passwordEncryptor)
        {
            clockService.Guard(nameof(clockService));
            userRepository.Guard(nameof(userRepository));
            passwordEncryptor.Guard(nameof(passwordEncryptor));

            _clockService = clockService;
            _userRepository = userRepository;
            _passwordEncryptor = passwordEncryptor;
        }


        protected override async Task<UpdateUserPasswordResponse> OnExecuteAsync(UpdateUserPasswordRequest tin, CancellationToken token = new CancellationToken())
        {
            User user = await _userRepository.GetUserByIdAsync(tin.UserId, token).ConfigureAwait(false);

            if (user == null)
                throw ServiceErrors.UserNotFoundException;

            if (user.Password != _passwordEncryptor.Encrypt(tin.OldPassword))
                return new UpdateUserPasswordResponse() { Result = UpdateUserPasswordResult.InvalidPassword };

            user.Password = _passwordEncryptor.Encrypt(tin.NewPassword);
            user.UpdatedAt = _clockService.Now();

            bool result = await _userRepository.UpdateUserAsync(user, token).ConfigureAwait(false);

            return new UpdateUserPasswordResponse()
                { Result = result ? UpdateUserPasswordResult.Success : UpdateUserPasswordResult.InternalServiceError };
        }
    }
}
