using Course_Work_Magazine.DTO.Basket_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class BasketQueryParamsValidator : AbstractValidator<BasketQueryParams>
{
    public BasketQueryParamsValidator()
    {
        RuleFor(b => b.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(b => b.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");

        RuleFor(b => b.Sort)
            .Must(s => string.IsNullOrWhiteSpace(s) ||
                       s.ToLower() == "productname" ||
                       s.ToLower() == "quantity")
            .WithMessage("Sort must be 'productName' or 'quantity'");

        RuleFor(b => b.SortDirection)
            .Must(sd => string.IsNullOrWhiteSpace(sd) || sd.ToLower() == "asc" || sd.ToLower() == "desc")
            .WithMessage("SortDirection must be 'asc' or 'desc'");
    }
}
