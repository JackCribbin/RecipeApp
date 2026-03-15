using FluentValidation;

public class RecipeRequestValidator : AbstractValidator<RecipeRequestDTO>
{
    public RecipeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than zero");

        RuleFor(x => x.Steps)
            .NotEmpty()
            .WithMessage("Recipe must have at least 1 step");

        RuleFor(x => x.RecipeIngredients)
            .NotEmpty()
            .WithMessage("Recipe must have at least 1 ingredient");

        RuleForEach(x => x.Steps)
            .SetValidator(new RecipeStepRequestValidator());

        RuleFor(x => x.Steps)
            .Must(steps =>
            {
                var sortedSteps = steps.OrderBy(s => s.StepNumber).ToList();
                for (int i = 0; i < sortedSteps.Count; i++)
                {
                    if (sortedSteps[i].StepNumber != i + 1)
                        return false;
                }

                return true;
            })
            .WithMessage("Steps must be continuous");

        RuleForEach(x => x.RecipeIngredients)
            .SetValidator(new RecipeIngredientRequestValidator());

        RuleForEach(x => x.Images)
            .SetValidator(new RecipeImageRequestValidator());

        RuleFor(x => x.Images)
            .Must(images => images.Count(i => i.IsPrimary) <= 1)
            .WithMessage("A recipe can only have one primary image");
    }
}

public class RecipeStepRequestValidator : AbstractValidator<RecipeStepRequestDTO>
{
    public RecipeStepRequestValidator()
    {
        RuleFor(x => x.StepNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Step number must be positive");

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .WithMessage("Instructions must not be empty")
            .MaximumLength(500)
            .WithMessage("Instructions must not exceed 500 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters");
    }
}

public class RecipeIngredientRequestValidator : AbstractValidator<RecipeIngredientRequestDTO>
{
    public RecipeIngredientRequestValidator()
    {
        RuleFor(x => x.IngredientId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A valid Ingredient Id is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be a positive amount");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters");
    }
}

public class RecipeImageRequestValidator : AbstractValidator<RecipeImageRequestDTO>
{
    public RecipeImageRequestValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage("Image Url must not be empty")
            .MaximumLength(100)
            .WithMessage("Image Url must not exceed 100 characters");

        RuleFor(x => x.Caption)
            .MaximumLength(200)
            .WithMessage("Caption must not exceed 200 characters");
    }
}