using Codelux.Common.Requests;

namespace Example.UserManagementService.Common.Requests
{
    public class UpdateUserPasswordRequest : AuthenticatedRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
    }
}
