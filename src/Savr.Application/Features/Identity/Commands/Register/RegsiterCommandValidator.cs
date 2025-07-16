using FluentValidation;

namespace Savr.Application.Features.Identity.Commands.Register
{
    public  class RegsiterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegsiterCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            // I copied this from https://stackoverflow.com/questions/63864594/how-can-i-create-strong-passwords-with-fluentvalidation
            RuleFor(x => x.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\@\.]+").WithMessage("Your password must contain at least one (!? @.).");
        }
    }
}
