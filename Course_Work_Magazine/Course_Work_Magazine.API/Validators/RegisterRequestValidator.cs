using Course_Work_Magazine.DTO.Auth_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MaximumLength(50).WithMessage("Name must be at most 50 characters");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.ConfirmedPassword).Equal(x => x.Password).WithMessage("Passwords must match");

        RuleFor(x => x.AvatarUrl).Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute)).WithMessage("AvatarUrl must be a valid URL").When(x => !string.IsNullOrEmpty(x.AvatarUrl));
    }
}