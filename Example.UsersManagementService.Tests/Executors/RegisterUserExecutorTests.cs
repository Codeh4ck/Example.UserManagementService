using Moq;
using System;
using NUnit.Framework;
using Codelux.Logging;
using Codelux.Mappers;
using System.Threading;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;
using Example.UserManagementService.Executors.RegisterUserExecutor;

namespace Example.UsersManagementService.Tests.Executors
{
    [TestFixture]
    public class RegisterUserExecutorTests
    {
        private Mock<ILogger> _loggerMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper<RegisterUserRequest, User>> _mapperMock;

        private RegisterUserExecutor _executor;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new();
            _userRepositoryMock = new();
            _mapperMock = new();

            _executor = new(_loggerMock.Object, _userRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public void GivenRegisterUserExecutorWhenInstantiatedWithNullLoggerThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RegisterUserExecutor(null, _userRepositoryMock.Object, _mapperMock.Object));
        }

        [Test]
        public void GivenRegisterUserExecutorWhenInstantiatedWithNullUserRepositoryThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RegisterUserExecutor(_loggerMock.Object, null, _mapperMock.Object));
        }

        [Test]
        public void GivenRegisterUserExecutorWhenInstantiatedWithNullMapperThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RegisterUserExecutor(_loggerMock.Object, _userRepositoryMock.Object, null));
        }

        [Test]
        public void GivenValidRegisterUserRequestWhenIExecuteThenUserIsCreated()
        {
            string username = "TestUser";
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");
            string email = "testemail@domain.com";

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = hashedPassword,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            RegisterUserRequest request = new()
            {
                Username = username,
                Password = password,
                Email = email
            };

            _mapperMock.Setup(x => x.Map(request)).Returns(user);
            _userRepositoryMock.Setup(x => x.CreateUserAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            RegisterUserResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(RegisterUserResult.Registered, result.Result);

            _mapperMock.Verify(x => x.Map(request), Times.Once);
            _userRepositoryMock.Verify(x => x.CreateUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GivenValidRegisterUserRequestWhenIExecuteAndRepositoryFailsThenUserIsNotCreated()
        {
            string username = "TestUser";
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");
            string email = "testemail@domain.com";

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = hashedPassword,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            RegisterUserRequest request = new()
            {
                Username = username,
                Password = password,
                Email = email
            };

            _mapperMock.Setup(x => x.Map(request)).Returns(user);
            _userRepositoryMock.Setup(x => x.CreateUserAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            RegisterUserResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(RegisterUserResult.InternalServiceError, result.Result);

            _mapperMock.Verify(x => x.Map(request), Times.Once);
            _userRepositoryMock.Verify(x => x.CreateUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
