public class Recipe
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
}