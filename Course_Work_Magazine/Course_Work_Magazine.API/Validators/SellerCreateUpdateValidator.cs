using Course_Work_Magazine.DTO.Customer_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class SellerCreateUpdateValidator : AbstractValidator<SellerCreateUpdateDto>
{
    public SellerCreateUpdateValidator()
    {
        RuleFor(s => s.Name).NotEmpty().WithMessage("Name is required").MinimumLength(2).WithMessage("Name must be at least 2 characters").MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(s => s.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email format");

        RuleFor(s => s.Address).MaximumLength(200).WithMessage("Address cannot exceed 200 characters").When(a => !string.IsNullOrEmpty(a.Address));

        RuleFor(s => s.PhoneNumber).Matches(@"^\+?[0-9]{7,15}$").WithMessage("Phone number must be valid").When(p => !string.IsNullOrEmpty(p.PhoneNumber));
    }
}
