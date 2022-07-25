using System.Data;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common.Models;

namespace Example.UserManagementService.Internal.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UserRepository(IDbConnectionFactory dbConnectionFactory)
        {
            dbConnectionFactory.Guard(nameof(dbConnectionFactory));
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> CreateUserAsync(User model, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.InsertAsync(model, false, false, token).ConfigureAwait(false) > 0;
        }

        public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.SingleAsync<User>(x => x.Id == userId, token).ConfigureAwait(false);
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.SingleAsync<User>(x => x.Email == email, token).ConfigureAwait(false);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.CountAsync<User>(x => x.Username == username, token).ConfigureAwait(false) == 0;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.CountAsync<User>(x => x.Email == email, token).ConfigureAwait(false) == 0;
        }

        public async Task<User> GetUserByCredentialsAsync(string usernameOrEmail, string password, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();

            return 
                await db.SingleAsync<User>(x => x.Username == usernameOrEmail && x.Password == password, token).ConfigureAwait(false) ??
                await db.SingleAsync<User>(x => x.Email == usernameOrEmail && x.Password == password, token).ConfigureAwait(false);
        }

        public async Task<bool> UpdateUserAsync(User model, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.UpdateAsync<User>(model, x => x.Id == model.Id, null, token).ConfigureAwait(false) > 0;
        }

        public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.DeleteAsync<User>(x => x.Id == userId, null, token).ConfigureAwait(false) > 0;
        }
    }
}
