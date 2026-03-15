using FluentValidation;

public class IngredientRequestValidator : AbstractValidator<IngredientRequestDTO>
{
    public IngredientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(30)
            .WithMessage("Unit must not exceed 30 characters");

        RuleFor(x => x.PricePerUnit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price per Unit cannot be a negative value");
    }
}