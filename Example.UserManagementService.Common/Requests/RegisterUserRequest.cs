using Codelux.Common.Requests;

namespace Example.UserManagementService.Common.Requests
{
    public class RegisterUserRequest : Request
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
