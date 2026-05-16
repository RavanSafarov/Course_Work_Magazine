using Course_Work_Magazine.DTO.Category_DTOs;
using FluentValidation;

namespace Course_Work_Magazine.Validators;

public class CategoryCreateUpdateValidator : AbstractValidator<CategoryCreateUpdateDto>
{
    public CategoryCreateUpdateValidator()
    {
        RuleFor(c => c.NameOfCategory)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(200).WithMessage("Category name cannot exceed 200 characters");
    }
}
