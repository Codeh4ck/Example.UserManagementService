using ServiceStack.Data;
using ServiceStack.OrmLite.Dapper;
using Codelux.Common.Extensions;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UserManagementService.Validators.ValidationRules
{
    public interface IEmailUniqueValidationRule
    {
        bool Matches(string email);
    }

    public class EmailUniqueValidationRule : IEmailUniqueValidationRule
    {
        private readonly IUserRepository _userRepository;

        public EmailUniqueValidationRule(IUserRepository userRepository)
        {
            userRepository.Guard(nameof(userRepository));
            _userRepository = userRepository;
        }

        public bool Matches(string email) => !string.IsNullOrEmpty(email) && _userRepository.IsEmailUniqueAsync(email).Result;
    }
}
