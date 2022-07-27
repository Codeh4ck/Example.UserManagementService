using NUnit.Framework;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;

namespace Example.UsersManagementService.Tests.DataAccess
{
    [TestFixture]
    public class InMemoryDatabaseTestFixture<TModel>
    {
        protected IDbConnectionFactory DbConnectionFactory;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            DbConnectionFactory = new OrmLiteConnectionFactory(":memory:", new SqliteOrmLiteDialectProvider());

            using var db = DbConnectionFactory.OpenDbConnection();
            db.CreateTableIfNotExists<TModel>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            using var db = DbConnectionFactory.OpenDbConnection();
            db.DropTable<TModel>();
        }
    }
}
