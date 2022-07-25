using Example.UserManagementService.Common.Models;

namespace Example.UserManagementService.Internal.DataAccess;

public interface IUserRepository
{
    Task<bool> CreateUserAsync(User model, CancellationToken token = default);
    Task<User> GetUserByIdAsync(Guid userId, CancellationToken token = default);
    Task<User> GetUserByEmailAsync(string email, CancellationToken token = default);
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken token = default);
    Task<User> GetUserByCredentialsAsync(string usernameOrEmail, string password, CancellationToken token = default);
    Task<bool> UpdateUserAsync(User model, CancellationToken token = default);
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken token = default);
}