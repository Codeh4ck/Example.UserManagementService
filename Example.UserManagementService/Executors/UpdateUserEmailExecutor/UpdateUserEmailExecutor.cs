using Codelux.Executors;
using Codelux.Utilities;
using Codelux.Utilities.Crypto;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Executors.UpdateUserEmailExecutor
{
    public class UpdateUserEmailExecutor : ExecutorBase<UpdateUserEmailRequest, UpdateUserEmailResponse>, IUpdateUserEmailExecutor
    {
        private readonly IClockService _clockService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncryptor _passwordEncryptor;

        public UpdateUserEmailExecutor(IClockService clockService, IUserRepository userRepository, IPasswordEncryptor passwordEncryptor)
        {
            clockService.Guard(nameof(clockService));
            userRepository.Guard(nameof(userRepository));
            passwordEncryptor.Guard(nameof(passwordEncryptor));

            _clockService = clockService;
            _userRepository = userRepository;
            _passwordEncryptor = passwordEncryptor;
        }


        protected override async Task<UpdateUserEmailResponse> OnExecuteAsync(UpdateUserEmailRequest tin, CancellationToken token = new CancellationToken())
        {
            bool isUnique = await _userRepository.IsEmailUniqueAsync(tin.NewEmail, token).ConfigureAwait(false);

            if (!isUnique)
                return new UpdateUserEmailResponse() { Result = UpdateUserEmailResult.EmailInUse };

            User user = await _userRepository.GetUserByIdAsync(tin.UserId, token).ConfigureAwait(false);

            if (user == null)
                throw ServiceErrors.UserNotFoundException;

            if (user.Password != _passwordEncryptor.Encrypt(tin.Password))
                return new UpdateUserEmailResponse() { Result = UpdateUserEmailResult.InvalidPassword };

            user.Email = tin.NewEmail;
            user.UpdatedAt = _clockService.Now();

            bool result = await _userRepository.UpdateUserAsync(user, token).ConfigureAwait(false);

            return new UpdateUserEmailResponse()
                { Result = result ? UpdateUserEmailResult.Success : UpdateUserEmailResult.InternalServiceError };
        }
    }
}
