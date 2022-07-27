using System;
using NUnit.Framework;
using System.Collections;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Validators;

namespace Example.UsersManagementService.Tests.Validators
{
    [TestFixture]
    public class UpdateUserPasswordRequestValidatorTests : ValidatorTestFixtureBase<UpdateUserPasswordRequestValidator, UpdateUserPasswordRequest>
    {
        protected static IEnumerable ValidTestCases = new[]
        {
            Case("GivenRequestIsValidWhenValidatingThenValidatorPasses", x => { })
        };

        protected static IEnumerable InvalidTestCases = new[]
        {
            Case("GivenUserIdIsEmptyWhenValidatingThenValidatorFails", x => x.UserId, x => x.UserId = Guid.Empty,
                "Please provide a valid user ID."),

            Case("GivenOldPasswordIsEmptyWhenValidatingThenValidatorFails", x => x.OldPassword,
                x => x.OldPassword = string.Empty, "Please enter your current password."),

            Case("GivenNewPasswordIsEmptyWhenValidatingThenValidatorFails", x => x.NewPassword,
                x => x.NewPassword = string.Empty, "Please enter your new password."),

            Case("GivenNewPasswordLengthIsNotAtLeast5WhenValidatingThenValidatorFails", x => x.NewPassword,
                x => x.NewPassword = "1234", "Password must be at least 5 characters long."),
            
            Case("GivenNewPasswordConfirmationIsEmptyWhenValidatingThenValidatorFails", x => x.NewPasswordConfirmation,
            x => x.NewPasswordConfirmation = string.Empty, "Please confirm your new password."),

            Case("GivenNewPasswordConfirmationIsInvalidWhenValidatingThenValidatorFails", x => x.NewPasswordConfirmation,
                x => 
                { 
                    x.NewPassword = "NewPassword123";
                    x.NewPasswordConfirmation = "NotPassword123";
                }, "Password confirmation does not match the provided password."),

        };

        protected override UpdateUserPasswordRequest CreateInput() =>
            new()
            {
                UserId = Guid.NewGuid(),
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                NewPasswordConfirmation = "NewPassword123"
            };

        protected override UpdateUserPasswordRequestValidator CreateValidator() => new();
    }
}
