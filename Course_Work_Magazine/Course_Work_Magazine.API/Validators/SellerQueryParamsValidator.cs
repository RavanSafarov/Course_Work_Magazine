using Course_Work_Magazine.DTO.Customer_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class SellerQueryParamsValidator : AbstractValidator<SellerQueryParams>
{
    public SellerQueryParamsValidator()
    {
        RuleFor(p => p.Page).GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(p => p.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(p => p.SortDirection).Must(x => x == null || x.ToLower() == "asc" || x.ToLower() == "desc").WithMessage("SortDirection must be 'asc' or 'desc'");
    }
}

