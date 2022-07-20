using Example.UserManagementService.Common.Model;

namespace Example.UserManagementService.Internal.DataAccess;

public interface IUserRepository
{
    Task<bool> CreateUserAsync(User model, CancellationToken token = default);
    Task<User> GetUserByIdAsync(Guid userId, CancellationToken token = default);
    Task<User> GetUserByCredentialsAsync(string username, string password, CancellationToken token = default);
    Task<bool> UpdateUserAsync(User model, CancellationToken token = default);
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken token = default);
}