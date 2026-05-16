using Course_Work_Magazine.DTO.Order_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class OrderQueryParamsValidator : AbstractValidator<OrderQueryParams>
{
    public OrderQueryParamsValidator()
    {
        RuleFor(c => c.Page).GreaterThan(0).WithMessage("Page must be greater than 0");
        RuleFor(c => c.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0").LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
        RuleFor(c => c.Sort).Must(s => string.IsNullOrWhiteSpace(s) || s.ToLower() == "startdate" || s.ToLower() == "enddate" || s.ToLower() == "createdat").WithMessage("Sort must be 'startDate', 'endDate' or 'createdAt'");
        RuleFor(c => c.SortDirection).Must(sd => string.IsNullOrWhiteSpace(sd) || sd.ToLower() == "asc" || sd.ToLower() == "desc").WithMessage("SortDirection must be 'asc' or 'desc'");
        RuleFor(c => c).Must(c => c.StartDateFrom == default || c.StartDateTo == default || c.StartDateFrom <= c.StartDateTo).WithMessage("StartDateFrom must be earlier than or equal to StartDateTo");
    }
}
