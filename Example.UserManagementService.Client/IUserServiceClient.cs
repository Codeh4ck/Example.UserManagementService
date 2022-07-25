using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;

namespace Example.UserManagementService.Client;

public interface IUserServiceClient
{
    Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request, CancellationToken token = default);
    Task<BasicUser> AuthenticateUserAsync(AuthenticateUserRequest request, CancellationToken token = default);
    Task<UpdateUserEmailResponse> UpdateUserEmailAsync(UpdateUserEmailRequest request, CancellationToken token = default);
    Task<UpdateUserPasswordResponse> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CancellationToken token = default);
}