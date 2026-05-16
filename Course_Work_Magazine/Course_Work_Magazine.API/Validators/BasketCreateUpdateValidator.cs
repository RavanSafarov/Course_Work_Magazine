using Course_Work_Magazine.DTO.Basket_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class BasketCreateUpdateValidator : AbstractValidator<BasketCreateUpdateDto>
{
    public BasketCreateUpdateValidator()
    {
        RuleFor(b => b.ProductId).GreaterThan(0).WithMessage("ProductId must be greater than 0");

        RuleFor(b => b.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1");
    }
}