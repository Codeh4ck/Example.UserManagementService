using ServiceStack.FluentValidation;
using Example.UserManagementService.Common.Requests;

namespace Example.UserManagementService.Validators
{
    public class AuthenticateUserRequestValidator : AbstractValidator<AuthenticateUserRequest>
    {
        public AuthenticateUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Please enter your username.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Please enter your password.");
        }
    }
}
