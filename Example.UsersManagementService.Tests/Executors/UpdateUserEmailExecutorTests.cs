using Moq;
using System;
using NUnit.Framework;
using System.Threading;
using Codelux.Utilities;
using Codelux.Common.Models;
using Codelux.Utilities.Crypto;
using Example.UserManagementService.Common;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;
using Example.UserManagementService.Executors.UpdateUserEmailExecutor;

namespace Example.UsersManagementService.Tests.Executors
{
    [TestFixture]
    public class UpdateUserEmailExecutorTests
    {
        private Mock<IClockService> _clockServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordEncryptor> _passwordEncryptorMock;

        private UpdateUserEmailExecutor _executor;

        [SetUp]
        public void Setup()
        {
            _clockServiceMock = new();
            _userRepositoryMock = new();
            _passwordEncryptorMock = new();

            _executor = new(_clockServiceMock.Object, _userRepositoryMock.Object, _passwordEncryptorMock.Object);
        }

        [Test]
        public void GivenUpdateUserEmailExecutorWhenInstantitatedWithNullClockServiceThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdateUserEmailExecutor(null, _userRepositoryMock.Object, _passwordEncryptorMock.Object));
        }

        [Test]
        public void GivenUpdateUserEmailExecutorWhenInstantitatedWithNullUserRepositoryThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdateUserEmailExecutor(_clockServiceMock.Object, null, _passwordEncryptorMock.Object));
        }

        [Test]
        public void GivenUpdateUserEmailExecutorWhenInstantitatedWithNullPasswordEncryptorThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdateUserEmailExecutor(_clockServiceMock.Object, _userRepositoryMock.Object, null));
        }

        [Test]
        public void GivenValidUpdateUserEmailRequestWhenIExecuteAndPasswordIsCorrectAndEmailIsUniqueThenEmailIsUpdated()
        {
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");
            string email = "testemail@domain.com";
            string newEmail = "newemail@domain.com";
            DateTime updatedAt = DateTime.Now;

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = hashedPassword,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            UpdateUserEmailRequest request = new()
            {
                UserId = user.Id,
                NewEmail = newEmail,
                NewEmailConfirmation = newEmail,
                Password = password
            };

            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(x => x.UpdateUserAsync(
                    It.Is<User>(
                        x => x.Id == user.Id && x.Email == request.NewEmail && x.UpdatedAt == updatedAt), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _passwordEncryptorMock.Setup(x => x.Encrypt(request.Password)).Returns(hashedPassword);
            _clockServiceMock.Setup(x => x.Now(It.IsAny<bool>())).Returns(updatedAt);

            UpdateUserEmailResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(UpdateUserEmailResult.Success, result.Result);

            _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()), Times.Once);

            _userRepositoryMock.Verify(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(
                    It.Is<User>(
                        x => x.Id == user.Id && x.Email == request.NewEmail && x.UpdatedAt == updatedAt),
                    It.IsAny<CancellationToken>()), Times.Once);

            _passwordEncryptorMock.Verify(x => x.Encrypt(request.Password), Times.Once);
            _clockServiceMock.Verify(x => x.Now(It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public void GivenValidUpdateUserEmailRequestWhenIExecuteAndPasswordIsCorrectAndEmailIsNotUniqueThenEmailIsNotUpdated()
        {
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");
            string email = "testemail@domain.com";
            string newEmail = "notunique@domain.com";
            DateTime updatedAt = DateTime.Now;

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = hashedPassword,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            UpdateUserEmailRequest request = new()
            {
                UserId = user.Id,
                NewEmail = newEmail,
                NewEmailConfirmation = newEmail,
                Password = password
            };

            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            UpdateUserEmailResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(UpdateUserEmailResult.EmailInUse, result.Result);

            _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()), Times.Once);

            _userRepositoryMock.Verify(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Never);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(
                    It.Is<User>(
                        x => x.Id == user.Id && x.Email == request.NewEmail && x.UpdatedAt == updatedAt),
                    It.IsAny<CancellationToken>()), Times.Never);

            _passwordEncryptorMock.Verify(x => x.Encrypt(request.Password), Times.Never);
            _clockServiceMock.Verify(x => x.Now(It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void GivenValidUpdateUserEmailRequestWhenIExecuteAndPasswordIsIncorrectAndEmailIsNotUniqueThenEmailIsNotUpdated()
        {
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");
            string email = "testemail@domain.com";
            string newEmail = "newemail@domain.com";

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = hashedPassword,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            UpdateUserEmailRequest request = new()
            {
                UserId = user.Id,
                NewEmail = newEmail,
                NewEmailConfirmation = newEmail,
                Password = password
            };

            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordEncryptorMock.Setup(x => x.Encrypt(request.Password)).Returns(Guid.NewGuid().ToString("N"));

            UpdateUserEmailResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(UpdateUserEmailResult.InvalidPassword, result.Result);

            _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()), Times.Once);

            _userRepositoryMock.Verify(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(
                    It.Is<User>(
                        x => x.Id == It.IsAny<Guid>() && x.Email == It.IsAny<string>() && x.UpdatedAt == It.IsAny<DateTime>()),
                    It.IsAny<CancellationToken>()), Times.Never);

            _passwordEncryptorMock.Verify(x => x.Encrypt(request.Password), Times.Once);
            _clockServiceMock.Verify(x => x.Now(It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void GivenValidUpdateUserEmailRequestWhenIExecuteUserDoesNotExistThenItThrows()
        {
            string password = "testPassword123";
            string newEmail = "newemail@domain.com";

            UpdateUserEmailRequest request = new()
            {
                UserId = Guid.NewGuid(),
                NewEmail = newEmail,
                NewEmailConfirmation = newEmail,
                Password = password
            };

            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(request.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            ServiceErrorException exception = Assert.ThrowsAsync<ServiceErrorException>(() => _executor.ExecuteAsync(request));

            Assert.IsNotNull(exception);
            Assert.AreEqual(ServiceErrors.UserNotFoundException, exception);

            _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(newEmail, It.IsAny<CancellationToken>()), Times.Once);

            _userRepositoryMock.Verify(x => x.GetUserByIdAsync(request.UserId, It.IsAny<CancellationToken>()), Times.Once);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(
                    It.Is<User>(
                        x => x.Id == It.IsAny<Guid>() && x.Email == It.IsAny<string>() && x.UpdatedAt == It.IsAny<DateTime>()),
                    It.IsAny<CancellationToken>()), Times.Never);

            _passwordEncryptorMock.Verify(x => x.Encrypt(request.Password), Times.Never);
            _clockServiceMock.Verify(x => x.Now(It.IsAny<bool>()), Times.Never);
        }
    }
}
