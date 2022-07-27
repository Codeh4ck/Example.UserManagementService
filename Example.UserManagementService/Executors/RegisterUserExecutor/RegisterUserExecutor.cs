using Codelux.Logging;
using Codelux.Mappers;
using Codelux.Executors;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Executors.RegisterUserExecutor
{
    public class RegisterUserExecutor : ExecutorBase<RegisterUserRequest, RegisterUserResponse>, IRegisterUserExecutor
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper<RegisterUserRequest, User> _requestToModelMapper;

        public RegisterUserExecutor(ILogger logger, IUserRepository userRepository, IMapper<RegisterUserRequest, User> requestToModelMapper)
        {
            logger.Guard(nameof(logger));
            userRepository.Guard(nameof(userRepository));
            requestToModelMapper.Guard(nameof(requestToModelMapper));

            _logger = logger;
            _userRepository = userRepository;
            _requestToModelMapper = requestToModelMapper;
        }

        protected override async Task<RegisterUserResponse> OnExecuteAsync(RegisterUserRequest tin, CancellationToken token = new())
        {
            User user = _requestToModelMapper.Map(tin);

            bool result = await _userRepository.CreateUserAsync(user, token).ConfigureAwait(false);

            if (result)
                _logger.LogEvent<RegisterUserExecutor>(LogType.Info,
                    $"Registered user with ID {user.Id} successfully!");
            else
                _logger.LogEvent<RegisterUserExecutor>(LogType.Error,
                    $"Could not register user with ID {user.Id}. Request ID: {tin.Id}");

            return new()
            {
                Id = user.Id,
                Result = result ? RegisterUserResult.Registered : RegisterUserResult.InternalServiceError
            };
        }
    }
}
