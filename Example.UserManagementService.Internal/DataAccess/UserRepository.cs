using System.Data;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Codelux.Common.Extensions;
using Example.UserManagementService.Common.Model;

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

        public async Task<User> GetUserByCredentialsAsync(string username, string password, CancellationToken token = default)
        {
            using IDbConnection db = _dbConnectionFactory.OpenDbConnection();
            return await db.SingleAsync<User>(x => x.Username == username && x.Password == password, token).ConfigureAwait(false);
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
