using Codelux.Common.Requests;

namespace Example.UserManagementService.Common.Requests
{
    public class AuthenticateUserRequest : Request
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
