using Course_Work_Magazine.DTO.Auth_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.UserName).MaximumLength(50).WithMessage("UserName must be at most 50 characters").When(x => !string.IsNullOrEmpty(x.UserName));

        RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid").When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.AvatarUrl).Must(x => string.IsNullOrEmpty(x) || Uri.IsWellFormedUriString(x, UriKind.Absolute)).WithMessage("AvatarUrl must be a valid URL").When(x => !string.IsNullOrEmpty(x.AvatarUrl));

        RuleFor(x => x.NewPassword).MinimumLength(6).WithMessage("NewPassword must be at least 6 characters").When(x => !string.IsNullOrEmpty(x.NewPassword));

        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("CurrentPassword is required when changing password").When(x => !string.IsNullOrEmpty(x.NewPassword));
    }
}
