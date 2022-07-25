using Codelux.Mappers;
using Codelux.Utilities;
using Codelux.Utilities.Crypto;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;

namespace Example.UserManagementService.Internal.Mappers
{
    public class RegisterUserRequestToUserMapper : MapperBase<RegisterUserRequest, User>
    {
        private readonly IClockService _clockService;
        private readonly IPasswordEncryptor _passwordEncryptor;

        public RegisterUserRequestToUserMapper(IClockService clockService, IPasswordEncryptor passwordEncryptor)
        {
            clockService.Guard(nameof(clockService));
            passwordEncryptor.Guard(nameof(passwordEncryptor));

            _clockService = clockService;
            _passwordEncryptor = passwordEncryptor;
        }

        public override User Map(RegisterUserRequest model)
        {
            return new User()
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                Password = _passwordEncryptor.Encrypt(model.Password),
                Email = model.Email,
                CreatedAt = _clockService.Now(),
                UpdatedAt = null
            };
        }
    }
}
