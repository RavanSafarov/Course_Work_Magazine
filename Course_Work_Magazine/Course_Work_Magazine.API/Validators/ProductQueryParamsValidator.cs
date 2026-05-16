using Course_Work_Magazine.DTO.Product_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class ProductQueryParamsValidator : AbstractValidator<ProductQueryParams>
{
    public ProductQueryParamsValidator()
    {
        RuleFor(p => p.Page).GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(p => p.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0").LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");

        RuleFor(p => p.Sort).Must(s => string.IsNullOrWhiteSpace(s) || s.ToLower() == "name" || s.ToLower() == "price").WithMessage("Sort must be 'name' or 'price'");

        RuleFor(p => p.SortDirection).Must(sd => string.IsNullOrWhiteSpace(sd) || sd.ToLower() == "asc" || sd.ToLower() == "desc").WithMessage("SortDirection must be 'asc' or 'desc'");
    }
}