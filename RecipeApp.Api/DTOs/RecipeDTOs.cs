public class RecipeRequestDTO
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Servings { get; set; }
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
    public ICollection<RecipeStep> Steps { get; set; } = [];
    public ICollection<RecipeImage> Images { get; set; } = [];
}

public class RecipeSummaryResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RecipeImage? Image { get; set; }

    public RecipeSummaryResponseDTO(Recipe recipe)
    {
        Id = recipe.Id;
        Name = recipe.Name;
        Description = recipe.Description;
        Image = recipe.Images.FirstOrDefault(i => i.IsPrimary);
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
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
    public ICollection<RecipeStep> Steps { get; set; } = [];
    public ICollection<RecipeImage> Images { get; set; } = [];

    public RecipeDetailsResponseDTO(Recipe recipe)
    {
        Id = recipe.Id;
        Name = recipe.Name;
        Description = recipe.Description;
        Servings = recipe.Servings;
        CreatedAt = recipe.CreatedAt;
        UpdatedAt = recipe.UpdatedAt;
        RecipeIngredients = recipe.RecipeIngredients;
        Steps = recipe.Steps;
        Images = recipe.Images;
    }
}