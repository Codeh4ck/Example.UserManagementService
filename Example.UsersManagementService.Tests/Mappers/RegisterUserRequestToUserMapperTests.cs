using Moq;
using System;
using NUnit.Framework;
using Codelux.Utilities;
using Codelux.Utilities.Crypto;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Internal.Mappers;

namespace Example.UsersManagementService.Tests.Mappers
{
    [TestFixture]
    public class RegisterUserRequestToUserMapperTests
    {
        private Mock<IClockService> _clockServiceMock;
        private Mock<IPasswordEncryptor> _passwordEncryptorMock;

        private RegisterUserRequestToUserMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _clockServiceMock = new();
            _passwordEncryptorMock = new();

            _mapper = new(_clockServiceMock.Object, _passwordEncryptorMock.Object);
        }

        [Test]
        public void GivenRegisterUserRequestToUserMapperWhenInstantiatedWithNullClockServiceThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestToUserMapper(null, _passwordEncryptorMock.Object));
        }

        [Test]
        public void GivenRegisterUserRequestToUserMapperWhenInstantiatedWithNullPasswordEncryptorThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new RegisterUserRequestToUserMapper(_clockServiceMock.Object, null));
        }

        [Test]
        public void GivenValidRegisterUserRequestWhenIMapThenUserModelIsReturned()
        {
            string password = Guid.NewGuid().ToString("N");

            RegisterUserRequest request = new()
            {
                Username = "Username",
                Password = "unhashedPassword",
                Email = "testuser@domain.com"
            };

            DateTime currentTime = DateTime.Now;

            _clockServiceMock.Setup(x => x.Now(false)).Returns(currentTime);
            _passwordEncryptorMock.Setup(x => x.Encrypt(request.Password)).Returns(password);

            User user = _mapper.Map(request);

            Assert.NotNull(user);
            Assert.AreEqual(request.Username, user.Username);
            Assert.AreEqual(request.Email, user.Email);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(currentTime, user.CreatedAt);
            Assert.AreEqual(null, user.UpdatedAt);

            _clockServiceMock.Verify(x => x.Now(false), Times.Once);
            _passwordEncryptorMock.Verify(x => x.Encrypt(request.Password), Times.Once);
        }
    }
}
