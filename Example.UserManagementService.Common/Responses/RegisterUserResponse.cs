using Example.UserManagementService.Common.Models;

namespace Example.UserManagementService.Common.Responses
{
    public class RegisterUserResponse
    {
        public Guid Id { get; set; }
        public RegisterUserResult Result { get; set; }
    }
}
