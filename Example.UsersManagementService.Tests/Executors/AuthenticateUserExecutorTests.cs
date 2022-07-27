using Moq;
using System;
using NUnit.Framework;
using System.Threading;
using Codelux.Mappers;
using Codelux.Common.Models;
using Codelux.Utilities.Crypto;
using Example.UserManagementService.Common;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;
using Example.UserManagementService.Executors.AuthenticateUserExecutor;

namespace Example.UsersManagementService.Tests.Executors
{
    [TestFixture]
    public class AuthenticateUserExecutorTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordEncryptor> _passwordEncryptorMock;
        private Mock<IMapper<User, BasicUser>> _mapperMock;
        
        private AuthenticateUserExecutor _executor;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new();
            _passwordEncryptorMock = new();
            _mapperMock = new();

            _executor = new(_userRepositoryMock.Object, _passwordEncryptorMock.Object, _mapperMock.Object);
        }

        [Test]
        public void GivenAuthenticateUserExecutorWhenInstantiatedWithNullUserRepositoryThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new AuthenticateUserExecutor(null, _passwordEncryptorMock.Object, _mapperMock.Object));
        }

        [Test]
        public void GivenAuthenticateUserExecutorWhenInstantiatedWithNullPasswordEncryptorThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new AuthenticateUserExecutor(_userRepositoryMock.Object, null, _mapperMock.Object));
        }

        [Test]
        public void GivenAuthenticateUserExecutorWhenInstantiatedWithNullMapperThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new AuthenticateUserExecutor(_userRepositoryMock.Object, _passwordEncryptorMock.Object, null));
        }

        [Test]
        public void GivenValidAuthenticateUserRequestWhenIExecuteAndUserExistsThenValidAuthenticateUserResponseIsReturned()
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

            BasicUser basicUser = new()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };

            AuthenticateUserRequest request = new()
            {
                Username = username,
                Password = password
            };

            _passwordEncryptorMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);

            _userRepositoryMock
                .Setup(x => x.GetUserByCredentialsAsync(user.Username, user.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mapperMock.Setup(x => x.Map(user)).Returns(basicUser);

            AuthenticateUserResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.User);

            Assert.AreEqual(user.Id, result.User.Id);
            Assert.AreEqual(user.Username, result.User.Username);
            Assert.AreEqual(user.Email, result.User.Email);
            Assert.AreEqual(user.CreatedAt, result.User.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, result.User.UpdatedAt);


            _passwordEncryptorMock.Verify(x => x.Encrypt(request.Password), Times.Once);
            _userRepositoryMock.Verify(x => x.GetUserByCredentialsAsync(user.Username, user.Password, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(x => x.Map(user), Times.Once);
        }

        [Test]
        public void GivenValidAuthenticateUserRequestWhenIExecuteAndUserDoesNotExistThenItThrows()
        {
            string username = "TestUser";
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");

            AuthenticateUserRequest request = new()
            {
                Username = username,
                Password = password
            };

            _passwordEncryptorMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);

            _userRepositoryMock
                .Setup(x => x.GetUserByCredentialsAsync(request.Username, hashedPassword, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            ServiceErrorException exception = Assert.ThrowsAsync<ServiceErrorException>(() => _executor.ExecuteAsync(request));

            Assert.AreEqual(ServiceErrors.InvalidCredentialsException, exception);

            _passwordEncryptorMock.Verify(x => x.Encrypt(request.Password), Times.Once);
            _userRepositoryMock.Verify(x => x.GetUserByCredentialsAsync(request.Username, hashedPassword, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(x => x.Map(It.IsAny<User>()), Times.Never);
        }
    }
}
