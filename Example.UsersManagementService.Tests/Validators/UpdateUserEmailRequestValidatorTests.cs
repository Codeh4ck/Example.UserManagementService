using System;
using NUnit.Framework;
using System.Collections;
using Example.UserManagementService.Validators;
using Example.UserManagementService.Common.Requests;

namespace Example.UsersManagementService.Tests.Validators
{
    [TestFixture]
    internal class UpdateUserEmailRequestValidatorTests : ValidatorTestFixtureBase<UpdateUserEmailRequestValidator, UpdateUserEmailRequest>
    {
        protected static IEnumerable ValidTestCases = new[]
        {
            Case("GivenRequestIsValidAndEmailIsUniqueWhenValidatingThenValidatorPasses", x => { })
        };

        protected static IEnumerable InvalidTestCases = new[]
        {
            Case("GivenUserIdIsEmptyWhenValidatingThenValidatorFails", x => x.UserId, x => x.UserId = Guid.Empty, "Please provide a valid user ID."),
            
            Case("GivenNewEmailIsEmptyWhenValidatingThenValidatorFails", x => x.NewEmail, x => x.NewEmail = string.Empty, "Please enter your desired e-mail address."),
            
            Case("GivenNewEmailIsInvalidWhenValidatingThenValidatorFails", x => x.NewEmail, x => x.NewEmail = "not an email", "Please enter a valid e-mail address."),
            
            Case("GivenNewEmailConfirmationIsEmptyWhenValidatingThenValidatorFails", x => x.NewEmailConfirmation, x => x.NewEmailConfirmation = string.Empty, "Please confirm your new e-mail address."),
            
            Case("GivenNewEmailConfirmationIsInvalidWhenValidatingThenValidatorFails",
                x => x.NewEmailConfirmation,
                x =>
                {
                    x.NewEmail = "testmail@domain.com";
                    x.NewEmailConfirmation = "different@domain.com";
                }, "E-mail address confirmation does not match the provided e-mail address."),

            Case("GivenPasswordIsEmptyWhenValidatingThenValidatorFails", x => x.Password, x => x.Password = "", "Please enter your current password."),
        };


        protected override UpdateUserEmailRequest CreateInput() =>
            new()
            {
                UserId = Guid.NewGuid(),
                Password = "Password123",
                NewEmail = "newtestemail@chargeleo.com",
                NewEmailConfirmation = "newtestemail@chargeleo.com"
            };

        protected override UpdateUserEmailRequestValidator CreateValidator() => new();
    }
}
