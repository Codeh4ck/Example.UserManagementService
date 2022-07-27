using ServiceStack.FluentValidation;
using Example.UserManagementService.Common.Requests;

namespace Example.UserManagementService.Validators
{
    public class UpdateUserEmailRequestValidator : AbstractValidator<UpdateUserEmailRequest>
    {
        public UpdateUserEmailRequestValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Please provide a valid user ID.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Please enter your current password.");

            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Please enter your desired e-mail address.")
                .EmailAddress().WithMessage("Please enter a valid e-mail address.");

            RuleFor(x => x.NewEmailConfirmation)
                .NotEmpty().WithMessage("Please confirm your new e-mail address.")
                .Matches(x => x.NewEmail).WithMessage("E-mail address confirmation does not match the provided e-mail address.");
        }
    }
}
