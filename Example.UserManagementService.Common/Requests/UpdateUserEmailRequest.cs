using Codelux.Common.Requests;

namespace Example.UserManagementService.Common.Requests
{
    public class UpdateUserEmailRequest : AuthenticatedRequest
    {
        public string Password { get; set; }
        public string NewEmail { get; set; }
        public string NewEmailConfirmation { get; set; }
    }
}
