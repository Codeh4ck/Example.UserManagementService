using Codelux.Mappers;
using Codelux.Executors;
using Codelux.Utilities.Crypto;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Executors.AuthenticateUserExecutor
{
    public class AuthenticateUserExecutor : ExecutorBase<AuthenticateUserRequest, AuthenticateUserResponse>, IAuthenticateUserExecutor
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly IMapper<User, BasicUser> _userToBasicUserMapper;

        public AuthenticateUserExecutor(
            IUserRepository userRepository,
            IPasswordEncryptor passwordEncryptor,
            IMapper<User, BasicUser> userToBasicUserMapper)
        {
            userRepository.Guard(nameof(userRepository));
            passwordEncryptor.Guard(nameof(passwordEncryptor));
            userToBasicUserMapper.Guard(nameof(userToBasicUserMapper));

            _userRepository = userRepository;
            _passwordEncryptor = passwordEncryptor;
            _userToBasicUserMapper = userToBasicUserMapper;
        }


        protected override async Task<AuthenticateUserResponse> OnExecuteAsync(AuthenticateUserRequest tin, CancellationToken token = new())
        {
            string hashedPassword = _passwordEncryptor.Encrypt(tin.Password);

            User user = await _userRepository.GetUserByCredentialsAsync(tin.Username, hashedPassword, token)
                .ConfigureAwait(false);

            if (user == null)
                throw ServiceErrors.InvalidCredentialsException;

            return new()
            {
                User = _userToBasicUserMapper.Map(user)
            };
        }
    }
}
