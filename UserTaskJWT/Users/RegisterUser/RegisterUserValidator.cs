using FluentValidation;

namespace UserTaskJWT.Web.Api.Users.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username can't be shorter than 3 characters")
                .MaximumLength(20).WithMessage("Username can't be longer than 20 characters");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be in valid format");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password can't be shorter than 8 characters")
                .MaximumLength(50).WithMessage("Password can't be longer than 50 characters")
                .Matches(@"[\!\?\*\.]+")
                    .WithMessage("Your password must contain at least one special symbol: (!? *.).");
        }
    }
}