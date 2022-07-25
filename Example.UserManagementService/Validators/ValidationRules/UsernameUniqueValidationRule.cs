using Codelux.Common.Extensions;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Validators.ValidationRules
{
    public interface IUsernameUniqueValidationRule
    {
        bool Matches(string username);
    }

    public class UsernameUniqueValidationRule : IUsernameUniqueValidationRule
    {
        private readonly IUserRepository _userRepository;

        public UsernameUniqueValidationRule(IUserRepository userRepository)
        {
            userRepository.Guard(nameof(userRepository));
            _userRepository = userRepository;
        }
        public bool Matches(string username) => !string.IsNullOrEmpty(username) && _userRepository.IsUsernameUniqueAsync(username).Result;
    }
}
