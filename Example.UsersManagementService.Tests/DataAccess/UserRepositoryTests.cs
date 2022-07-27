using System;
using NUnit.Framework;
using ServiceStack.OrmLite;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Internal.DataAccess;

namespace Example.UsersManagementService.Tests.DataAccess
{
    [TestFixture]
    public class UserRepositoryTests : InMemoryDatabaseTestFixture<User>
    {
        private IUserRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new UserRepository(DbConnectionFactory);
        }

        [TearDown]
        public void TearDown()
        {
            using var db = DbConnectionFactory.OpenDbConnection();
            db.DeleteAllAsync<User>();
        }

        [Test]
        public void GivenUsersRepositoryWhenInstantiatedWithNullDbConnectionFactoryThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new UserRepository(null));
        }

        [Test]
        public void GivenUserModelWhenICreateUserThenUserIsCreated()
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = Guid.NewGuid().ToString("N"),
                Email = "testmail@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;

            Assert.IsTrue(result);

            User retrieved = _repository.GetUserByIdAsync(user.Id).Result;

            Assert.IsNotNull(retrieved);
            Assert.AreEqual(user.Id, retrieved.Id);
            Assert.AreEqual(user.Username, retrieved.Username);
            Assert.AreEqual(user.Password, retrieved.Password);
            Assert.AreEqual(user.Email, retrieved.Email);
            Assert.AreEqual(user.CreatedAt, retrieved.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, retrieved.UpdatedAt);
        }

        [Test]
        public void GivenUserExistsWhenIGetUserByIdThenUserIsReturned()
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = Guid.NewGuid().ToString("N"),
                Email = "testmail@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;

            Assert.IsTrue(result);

            User retrieved = _repository.GetUserByIdAsync(user.Id).Result;

            Assert.IsNotNull(retrieved);
            Assert.AreEqual(user.Id, retrieved.Id);
            Assert.AreEqual(user.Username, retrieved.Username);
            Assert.AreEqual(user.Password, retrieved.Password);
            Assert.AreEqual(user.Email, retrieved.Email);
            Assert.AreEqual(user.CreatedAt, retrieved.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, retrieved.UpdatedAt);
        }

        [Test]
        public void GivenUserExistsWhenIGetUserByCredentialsThenUserIsReturned()
        {
            string username = "TestUser";
            string email = "testmail@domain.com";
            string password = Guid.NewGuid().ToString("N");

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = password,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;

            Assert.IsTrue(result);

            User retrieved = _repository.GetUserByCredentialsAsync(username, password).Result;

            Assert.IsNotNull(retrieved);
            Assert.AreEqual(user.Id, retrieved.Id);
            Assert.AreEqual(user.Username, retrieved.Username);
            Assert.AreEqual(user.Password, retrieved.Password);
            Assert.AreEqual(user.Email, retrieved.Email);
            Assert.AreEqual(user.CreatedAt, retrieved.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, retrieved.UpdatedAt);

            retrieved = _repository.GetUserByCredentialsAsync(email, password).Result;

            Assert.IsNotNull(retrieved);
            Assert.AreEqual(user.Id, retrieved.Id);
            Assert.AreEqual(user.Username, retrieved.Username);
            Assert.AreEqual(user.Password, retrieved.Password);
            Assert.AreEqual(user.Email, retrieved.Email);
            Assert.AreEqual(user.CreatedAt, retrieved.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, retrieved.UpdatedAt);
        }

        [Test]
        public void GivenUserExistsWhenIGetUserByEmailThenUserIsReturned()
        {
            string email = "testmail@domain.com";

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = Guid.NewGuid().ToString("N"),
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;

            Assert.IsTrue(result);

            User retrieved = _repository.GetUserByEmailAsync(user.Email).Result;

            Assert.IsNotNull(retrieved);
            Assert.AreEqual(user.Id, retrieved.Id);
            Assert.AreEqual(user.Username, retrieved.Username);
            Assert.AreEqual(user.Password, retrieved.Password);
            Assert.AreEqual(user.Email, retrieved.Email);
            Assert.AreEqual(user.CreatedAt, retrieved.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, retrieved.UpdatedAt);
        }


        [Test]
        public void GivenUserExistsWhenIUpdateUserThenUserIsUpdated()
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = Guid.NewGuid().ToString("N"),
                Email = "testuser@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;

            Assert.IsTrue(result);

            user.Username = "NewUsername";
            user.Email = "newemail@domain.com";

            result = _repository.UpdateUserAsync(user).Result;
            Assert.IsTrue(result);

            User retrieved = _repository.GetUserByIdAsync(user.Id).Result;

            Assert.IsNotNull(retrieved);

            Assert.AreEqual(user.Id, retrieved.Id);
            Assert.AreEqual("NewUsername", retrieved.Username);
            Assert.AreEqual(user.Password, retrieved.Password);
            Assert.AreEqual("newemail@domain.com", retrieved.Email);
            Assert.AreEqual(user.CreatedAt, retrieved.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, retrieved.UpdatedAt);
        }

        [Test]
        public void GivenUserExistsWhenIDeleteUserThenUserIsDeleted()
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = Guid.NewGuid().ToString("N"),
                Email = "testuser@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;
            Assert.IsTrue(result);

            result = _repository.DeleteUserAsync(user.Id).Result;
            Assert.IsTrue(result);

            User retrieved = _repository.GetUserByIdAsync(user.Id).Result;
            Assert.IsNull(retrieved);
        }

        [Test]
        public void GivenUserExistsWhenICheckIfItsUsernameIsUniqueThenFalseIsReturned()
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = Guid.NewGuid().ToString("N"),
                Email = "testuser@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;
            Assert.IsTrue(result);

            bool isUnique = _repository.IsUsernameUniqueAsync(user.Username).Result;

            Assert.IsFalse(isUnique);
        }

        [Test]
        public void GivenUserDoesNotExistWhenICheckIfItsUsernameIsUniqueThenTrueIsReturned()
        {
            bool isUnique = _repository.IsUsernameUniqueAsync(Guid.NewGuid().ToString("N").Substring(0, 10)).Result;
            Assert.IsTrue(isUnique);
        }

        [Test]
        public void GivenUserExistsWhenICheckIfItsEmailIsUniqueThenFalseIsReturned()
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = Guid.NewGuid().ToString("N"),
                Email = "testuser@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            bool result = _repository.CreateUserAsync(user).Result;
            Assert.IsTrue(result);

            bool isUnique = _repository.IsEmailUniqueAsync(user.Email).Result;

            Assert.IsFalse(isUnique);
        }

        [Test]
        public void GivenUserDoesNotExistWhenICheckIfItsEmailIsUniqueThenTrueIsReturned()
        {
            bool isUnique = _repository.IsEmailUniqueAsync($"{Guid.NewGuid().ToString().Substring(0, 10)}@domain.com").Result;
            Assert.IsTrue(isUnique);
        }

        [Test]
        public void GivenUserDoesNotExistWhenIGetUserByAnyPropertyThenNullIsReturned()
        {
            User user = _repository.GetUserByIdAsync(Guid.NewGuid()).Result;
            Assert.IsNull(user);

            user = _repository.GetUserByCredentialsAsync("SomeUsername", Guid.NewGuid().ToString("N")).Result;
            Assert.IsNull(user);

            user = _repository.GetUserByEmailAsync("nonexistent@chargeleo.com").Result;
            Assert.IsNull(user);
        }
    }
}
