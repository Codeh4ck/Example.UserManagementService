using Moq;
using System;
using NUnit.Framework;
using System.Threading;
using Codelux.Common.Models;
using Codelux.Utilities;
using Codelux.Utilities.Crypto;
using Example.UserManagementService.Common;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Common.Responses;
using Example.UserManagementService.Internal.DataAccess;
using Example.UserManagementService.Executors.UpdateUserPasswordExecutor;

namespace Example.UsersManagementService.Tests.Executors
{
    [TestFixture]
    public class UpdateUserPasswordExecutorTests
    {
        private Mock<IClockService> _clockServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordEncryptor> _passwordEncryptorMock;

        private UpdateUserPasswordExecutor _executor;

        [SetUp]
        public void Setup()
        {
            _clockServiceMock = new();
            _userRepositoryMock = new();
            _passwordEncryptorMock = new();

            _executor = new(_clockServiceMock.Object, _userRepositoryMock.Object, _passwordEncryptorMock.Object);
        }

        [Test]
        public void GivenUpdateUserPasswordExecutorWhenInstantitatedWithNullClockServiceThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdateUserPasswordExecutor(null, _userRepositoryMock.Object, _passwordEncryptorMock.Object));
        }

        [Test]
        public void GivenUpdateUserPasswordExecutorWhenInstantitatedWithNullUserRepositoryThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdateUserPasswordExecutor(_clockServiceMock.Object, null, _passwordEncryptorMock.Object));
        }

        [Test]
        public void GivenUpdateUserPasswordExecutorWhenInstantitatedWithNullPasswordEncryptorThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdateUserPasswordExecutor(_clockServiceMock.Object, _userRepositoryMock.Object, null));
        }

        [Test]
        public void GivenValidUpdateUserPasswordRequestWhenIExecuteThenPasswordIsUpdated()
        {
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");
            string newPassword = "newPassword123";
            string hashedNewPassword = Guid.NewGuid().ToString("N");
            DateTime updatedAt = DateTime.Now;

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = hashedPassword,
                Email = "testemail@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            UpdateUserPasswordRequest request = new()
            {
                UserId = user.Id,
                OldPassword = password,
                NewPassword = newPassword,
                NewPasswordConfirmation = newPassword
            };

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordEncryptorMock.Setup(x => x.Encrypt(password)).Returns(hashedPassword);
            _passwordEncryptorMock.Setup(x => x.Encrypt(newPassword)).Returns(hashedNewPassword);

            _clockServiceMock.Setup(x => x.Now(It.IsAny<bool>())).Returns(updatedAt);

            _userRepositoryMock.Setup(x => x.UpdateUserAsync(
                It.Is<User>(
                    x => x.Id == user.Id && x.Password == hashedNewPassword && x.UpdatedAt == updatedAt
                ), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            UpdateUserPasswordResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(UpdateUserPasswordResult.Success, result.Result);

            _userRepositoryMock.Verify(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);

            _passwordEncryptorMock.Verify(x => x.Encrypt(password), Times.Once);
            _passwordEncryptorMock.Verify(x => x.Encrypt(newPassword), Times.Once);

            _clockServiceMock.Verify(x => x.Now(It.IsAny<bool>()), Times.Once);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(
                It.Is<User>(
                    x => x.Id == user.Id && x.Password == hashedNewPassword && x.UpdatedAt == updatedAt
                ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GivenValidUpdateUserPasswordRequestWhenIExecuteAndOldPasswordIsIncorrectThenPasswordIsUpdated()
        {
            string password = "testPassword123";
            string hashedPassword = Guid.NewGuid().ToString("N");
            string newPassword = "newPassword123";

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "TestUser",
                Password = hashedPassword,
                Email = "testemail@domain.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            UpdateUserPasswordRequest request = new()
            {
                UserId = user.Id,
                OldPassword = password,
                NewPassword = newPassword,
                NewPasswordConfirmation = newPassword
            };

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordEncryptorMock.Setup(x => x.Encrypt(password)).Returns(Guid.NewGuid().ToString("N"));


            UpdateUserPasswordResponse result = _executor.ExecuteAsync(request).Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(UpdateUserPasswordResult.InvalidPassword, result.Result);

            _userRepositoryMock.Verify(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);

            _passwordEncryptorMock.Verify(x => x.Encrypt(password), Times.Once);
            _passwordEncryptorMock.Verify(x => x.Encrypt(newPassword), Times.Never);

            _clockServiceMock.Verify(x => x.Now(It.IsAny<bool>()), Times.Never);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(
                It.Is<User>(
                    x => x.Id == user.Id && x.Password == It.IsAny<string>() && x.UpdatedAt == It.IsAny<DateTime>()
                ), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void GivenValidUpdateUserPasswordRequestWhenIExecuteAndUserDoesNotExistThenItThrows()
        {
            string password = "testPassword123";
            string newPassword = "newPassword123";

            UpdateUserPasswordRequest request = new()
            {
                UserId = Guid.NewGuid(),
                OldPassword = password,
                NewPassword = newPassword,
                NewPasswordConfirmation = newPassword
            };

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(request.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);


            ServiceErrorException exception = Assert.ThrowsAsync<ServiceErrorException>(() => _executor.ExecuteAsync(request));

            Assert.IsNotNull(exception);
            Assert.AreEqual(ServiceErrors.UserNotFoundException, exception);

            _userRepositoryMock.Verify(x => x.GetUserByIdAsync(request.UserId, It.IsAny<CancellationToken>()), Times.Once);

            _passwordEncryptorMock.Verify(x => x.Encrypt(password), Times.Never);
            _passwordEncryptorMock.Verify(x => x.Encrypt(newPassword), Times.Never);

            _clockServiceMock.Verify(x => x.Now(It.IsAny<bool>()), Times.Never);

            _userRepositoryMock.Verify(x => x.UpdateUserAsync(
                It.Is<User>(
                    x => x.Id == It.IsAny<Guid>() && x.Password == It.IsAny<string>() && x.UpdatedAt == It.IsAny<DateTime>()
                ), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
