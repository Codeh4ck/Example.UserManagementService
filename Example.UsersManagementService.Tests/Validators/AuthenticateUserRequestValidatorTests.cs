using NUnit.Framework;
using System.Collections;
using Example.UserManagementService.Validators;
using Example.UserManagementService.Common.Requests;

namespace Example.UsersManagementService.Tests.Validators
{
    [TestFixture]
    public class AuthenticateUserRequestValidatorTests : ValidatorTestFixtureBase<AuthenticateUserRequestValidator, AuthenticateUserRequest>
    {
        protected static IEnumerable ValidTestCases = new[]
        {
            Case("GivenRequestIsValidWhenValidatingThenValidatorPasses", x => { })
        };

        protected static IEnumerable InvalidTestCases = new[]
        {
            Case("GivenUsernameIsEmptyWhenValidatingThenValidatorFails", x => x.Username,
                x => x.Username = string.Empty, "Please enter your username."),

            Case("GivenPasswordIsEmptyWhenValidatingThenValidatorFails", x => x.Password,
                x => x.Password = string.Empty, "Please enter your password."),
        };

        protected override AuthenticateUserRequest CreateInput() =>
            new()
            {
                Username = "TestUser",
                Password = "TestPassword"
            };

        protected override AuthenticateUserRequestValidator CreateValidator() => new();
    }
}
