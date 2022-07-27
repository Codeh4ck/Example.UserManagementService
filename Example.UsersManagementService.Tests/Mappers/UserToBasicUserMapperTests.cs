using System;
using NUnit.Framework;
using Example.UserManagementService.Common.Models;
using Example.UserManagementService.Internal.Mappers;

namespace Example.UsersManagementService.Tests.Mappers
{
    [TestFixture]
    public class UserToBasicUserMapperTests
    {
        private UserToBasicUserMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = new();
        }

        [Test]
        public void GivenUserWhenIMapThenBasicUserIsReturned()
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = "Username",
                Password = Guid.NewGuid().ToString("N"),
                Email = "testuser@domain.com",
                CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                UpdatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2))
            };


            BasicUser basicUser = _mapper.Map(user);

            Assert.NotNull(basicUser);
            Assert.AreEqual(user.Id, basicUser.Id);
            Assert.AreEqual(user.Username, basicUser.Username);
            Assert.AreEqual(user.Email, basicUser.Email);
            Assert.AreEqual(user.CreatedAt, basicUser.CreatedAt);
            Assert.AreEqual(user.UpdatedAt, basicUser.UpdatedAt);
        }
    }
}
