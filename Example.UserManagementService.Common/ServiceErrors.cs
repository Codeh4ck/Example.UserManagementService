using System.Net;
using Codelux.Common.Models;

namespace Example.UserManagementService.Common
{
    public static class ServiceErrors
    {
        public static readonly ServiceErrorException InvalidCredentialsException =
            new(ServiceConstants.ServiceName, 0, HttpStatusCode.NotFound,
                "Username or password is not correct.");  
        
        public static readonly ServiceErrorException UserNotFoundException =
            new(ServiceConstants.ServiceName, 1, HttpStatusCode.NotFound,
                "User not found.");
    }
}
