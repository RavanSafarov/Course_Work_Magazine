using Course_Work_Magazine.DTO.Product_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class ProductCreateUpdateValidator : AbstractValidator<ProductCreateUpdateDto>
{
    public ProductCreateUpdateValidator()
    {
        RuleFor(p => p.NameOfProduct).NotEmpty().WithMessage("Product name is required").MaximumLength(250).WithMessage("Product name cannot exceed 250 characters");

        RuleFor(p => p.Description).MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(p => p.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

        RuleFor(p => p.ImageUrl).MaximumLength(1000).WithMessage("Image URL cannot exceed 1000 characters");

        RuleFor(p => p.CategoryId).GreaterThan(0).WithMessage("CategoryId must be greater than 0");

        RuleFor(p => p.SellerId).GreaterThan(0).WithMessage("SellerId must be greater than 0");
    }
}
