using System.Diagnostics.CodeAnalysis;

public class RecipeRequestDTO
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Servings { get; set; }
    public ICollection<RecipeIngredientResponseDTO> RecipeIngredients { get; set; } = [];
    public ICollection<RecipeStepResponseDTO> Steps { get; set; } = [];
    public ICollection<RecipeImageResponseDTO> Images { get; set; } = [];
}

public class RecipeSummaryResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RecipeImageResponseDTO? Image { get; set; }

    public RecipeSummaryResponseDTO(Recipe recipe)
    {
        Id = recipe.Id;
        Name = recipe.Name;
        Description = recipe.Description;

        var primaryRecipeImage = recipe.Images.FirstOrDefault(i => i.IsPrimary);
        if(primaryRecipeImage != null)
            Image = new RecipeImageResponseDTO(primaryRecipeImage);
    }
}

public class RecipeDetailsResponseDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Servings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<RecipeIngredientResponseDTO> RecipeIngredients { get; set; } = [];
    public ICollection<RecipeStepResponseDTO> Steps { get; set; } = [];
    public ICollection<RecipeImageResponseDTO> Images { get; set; } = [];

    public RecipeDetailsResponseDTO(Recipe recipe)
    {
        Id = recipe.Id;
        Name = recipe.Name;
        Description = recipe.Description;
        Servings = recipe.Servings;
        CreatedAt = recipe.CreatedAt;
        UpdatedAt = recipe.UpdatedAt;
        RecipeIngredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientResponseDTO(ri)).ToArray();
        Steps = recipe.Steps.Select(s => new RecipeStepResponseDTO(s)).ToArray();
        Images = recipe.Images.Select(i => new RecipeImageResponseDTO(i)).ToArray();
    }
}

public class RecipeIngredientResponseDTO
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string IngredientUnit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }

    public RecipeIngredientResponseDTO(RecipeIngredient recipeIngredient)
    {
        Id = recipeIngredient.Id;
        IngredientId = recipeIngredient.IngredientId;
        IngredientName = recipeIngredient.Ingredient.Name;
        IngredientUnit = recipeIngredient.Ingredient.Unit;
        Quantity = recipeIngredient.Quantity;
        Notes = recipeIngredient.Notes;
    }
}

public class RecipeStepResponseDTO
{
    public int Id { get; set; }
    public int StepNumber { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public RecipeStepResponseDTO(RecipeStep recipeStep)
    {
        Id = recipeStep.Id;
        StepNumber = recipeStep.StepNumber;
        Instructions = recipeStep.Instructions;
        Notes = recipeStep.Notes;
    }
}

public class RecipeImageResponseDTO
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }

    public RecipeImageResponseDTO(RecipeImage recipeImage)
    {
        ImageUrl = recipeImage.ImageUrl;
        Caption = recipeImage.Caption;
        IsPrimary = recipeImage.IsPrimary;
    }
}