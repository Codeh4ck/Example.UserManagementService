using ServiceStack.FluentValidation;
using Example.UserManagementService.Common.Requests;

namespace Example.UserManagementService.Validators
{
    public class UpdateUserPasswordRequestValidator : AbstractValidator<UpdateUserPasswordRequest>
    {
        public UpdateUserPasswordRequestValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Please provide a valid user ID.");

            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Please enter your current password.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Please enter your new password.")
                .MinimumLength(5).WithMessage("Password must be at least 5 characters long.");

            RuleFor(x => x.NewPasswordConfirmation)
                .NotEmpty().WithMessage("Please confirm your new password.")
                .Matches(x => x.NewPassword).WithMessage("Password confirmation does not match the provided password.");
        }
    }
}
