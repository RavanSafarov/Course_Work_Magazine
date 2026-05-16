using Course_Work_Magazine.DTO.Order_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class OrderCreateUpdateValidator : AbstractValidator<OrderCreateUpdateDto>
{
    public OrderCreateUpdateValidator()
    {
        RuleFor(i => i.SellerId).NotEmpty().WithMessage("SellerId is required").GreaterThan(0).WithMessage("SellerId must be greater than 0");

        RuleFor(i => i.Comment).NotEmpty().WithMessage("Order Comment is required").MinimumLength(5).When(c => !string.IsNullOrEmpty(c.Comment)).WithMessage("Order Comment must be at least 5 characters long");

        RuleFor(i => i.StartDate).NotEmpty().WithMessage("Order Start Date is required");

        RuleFor(i => i.EndDate).NotEmpty().WithMessage("Order End Date is required");

        RuleFor(i => i).Must(x => x.EndDate > x.StartDate).WithMessage("EndDate must be later than StartDate");

        RuleFor(i => i.Items).NotEmpty().WithMessage("Order must contain at least one item");

    }
}
