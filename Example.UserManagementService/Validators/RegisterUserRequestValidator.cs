using Codelux.Common.Extensions;
using ServiceStack.FluentValidation;
using Example.UserManagementService.Common.Requests;
using Example.UserManagementService.Validators.ValidationRules;

namespace Example.UserManagementService.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator(IUsernameUniqueValidationRule usernameUniqueValidationRule,
            IEmailUniqueValidationRule emailUniqueValidationRule)
        {
            usernameUniqueValidationRule.Guard(nameof(usernameUniqueValidationRule));
            emailUniqueValidationRule.Guard(nameof(emailUniqueValidationRule));

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Please enter your desired username.")
                .Length(4, 150).WithMessage("Username must be 4 to 150 characters long.")
                .Must(usernameUniqueValidationRule.Matches).WithMessage("Username is in use. Please choose a different username.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Please enter your desired password.")
                .MinimumLength(5).WithMessage("Password must be at least 5 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Please enter your e-mail address.")
                .EmailAddress().WithMessage("Please enter a valid e-mail address")
                .Must(emailUniqueValidationRule.Matches).WithMessage("E-mail address is in use. Please choose a different e-mail address.");
        }
    }
}
