using Moq;
using System;
using System.Text;
using NUnit.Framework;
using System.Collections;
using ServiceStack.FluentValidation.Results;
using Example.UserManagementService.Validators;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Validators.ValidationRules;

namespace Example.UsersManagementService.Tests.Validators
{
    [TestFixture]
    public class RegisterUserRequestValidatorTests : ValidatorTestFixtureBase<RegisterUserRequestValidator, RegisterUserRequest>
    {
        private Mock<IEmailUniqueValidationRule> _emailUniqueValidationRuleMock;
        private Mock<IUsernameUniqueValidationRule> _usernameUniqueValidationRuleMock;

        [Test]
        public void GivenRegisterUserValidatorWhenInstantiatedWithNullUsernameUniqueValidationRuleThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RegisterUserRequestValidator(null, _emailUniqueValidationRuleMock.Object));
        }

        [Test]
        public void GivenRegisterUserValidatorWhenInstantiatedWithNullEmailUniqueValidationRuleThenItThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RegisterUserRequestValidator(_usernameUniqueValidationRuleMock.Object, null));
        }

        protected static IEnumerable ValidTestCases = new[]
        {
            Case("GivenRequestIsValidWhenValidatingThenValidatorPasses", x => { })
        };

        protected static IEnumerable InvalidTestCases = new[]
        {
            Case("GivenEmptyUsernameWhenValidatingThenValidatorFails", x => x.Username, x => x.Username = string.Empty,
                "Please enter your desired username."),

            Case("GivenUsernameLengthLessThan4CharactersWhenValidatingThenValidatorFails", x => x.Username,
                x => x.Username = "Ab", "Username must be 4 to 150 characters long."),

            Case("GivenUsernameLengthMoreThan150CharactersWhenValidatingThenValidatorFails", x => x.Username,
                x => x.Username = CreateLongUsername(), "Username must be 4 to 150 characters long."),

            Case("GivenEmptyPasswordWhenValidatingThenValidatorFails", x => x.Password, x => x.Password = string.Empty,
                "Please enter your desired password."),

            Case("GivenPasswordLengthIsNotAtLeast5WhenValidatingThenValidatorFails", x => x.Password,
                x => x.Password = "1234", "Password must be at least 5 characters long."),

            Case("GivenEmptyEmailWhenValidatingThenValidatorFails", x => x.Email, x => x.Email = string.Empty,
                "Please enter your e-mail address."),

            Case("GivenInvalidEmailWhenValidatingThenValidatorFails", x => x.Email, x => x.Email = "not an email",
                "Please enter a valid e-mail address."),

        };

        [Test]
        public void GivenEmailAddressIsNotUniqueWhenValidatingThenValidatorFails()
        {
            Mock<IUsernameUniqueValidationRule> usernameUniqueRuleMock = new();
            Mock<IEmailUniqueValidationRule> emailUniqueRuleMock = new();

            RegisterUserRequestValidator validator = new(usernameUniqueRuleMock.Object, emailUniqueRuleMock.Object);

            string username = "TestUser";
            string email = "nonunique@domain.com";

            usernameUniqueRuleMock.Setup(x => x.Matches(username)).Returns(true);
            emailUniqueRuleMock.Setup(x => x.Matches(email)).Returns(false);

            RegisterUserRequest request = new()
            {
                Email = email,
                Username = username,
                Password = "Password123"
            };

            ValidationResult result = validator.Validate(request);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("E-mail address is in use. Please choose a different e-mail address.", result.Errors[0].ErrorMessage);

            usernameUniqueRuleMock.Verify(x => x.Matches(username), Times.Once);
            emailUniqueRuleMock.Verify(x => x.Matches(email), Times.Once);
        }

        [Test]
        public void GivenUsernameIsNotUniqueWhenValidatingThenValidatorFails()
        {
            Mock<IUsernameUniqueValidationRule> usernameUniqueRuleMock = new();
            Mock<IEmailUniqueValidationRule> emailUniqueRuleMock = new();

            RegisterUserRequestValidator validator = new(usernameUniqueRuleMock.Object, emailUniqueRuleMock.Object);

            string username = "TestUser";
            string email = "unique@domain.com";

            usernameUniqueRuleMock.Setup(x => x.Matches(username)).Returns(false);
            emailUniqueRuleMock.Setup(x => x.Matches(email)).Returns(true);

            RegisterUserRequest request = new()
            {
                Email = email,
                Username = username,
                Password = "Password123"
            };

            ValidationResult result = validator.Validate(request);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Errors);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("Username is in use. Please choose a different username.", result.Errors[0].ErrorMessage);

            usernameUniqueRuleMock.Verify(x => x.Matches(username), Times.Once);
            emailUniqueRuleMock.Verify(x => x.Matches(email), Times.Once);
        }

        private static string CreateLongUsername()
        {
            StringBuilder longUsernameBuilder = new();
            for (int x = 0; x < 151; x++)
                longUsernameBuilder.Append("a");

            return longUsernameBuilder.ToString();
        }

        protected override RegisterUserRequest CreateInput() =>
            new()
            {
                Email = "testuser@domain.com",
                Username = "TestUser",
                Password = "Password123"
            };

        protected override RegisterUserRequestValidator CreateValidator()
        {
            _usernameUniqueValidationRuleMock = new();
            _emailUniqueValidationRuleMock = new();

            _usernameUniqueValidationRuleMock.Setup(x => x.Matches(It.IsAny<string>())).Returns(true);
            _emailUniqueValidationRuleMock.Setup(x => x.Matches(It.IsAny<string>())).Returns(true);

            return new(_usernameUniqueValidationRuleMock.Object, _emailUniqueValidationRuleMock.Object);
        }
    }
}
