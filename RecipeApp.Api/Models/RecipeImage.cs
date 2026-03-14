public class RecipeImage
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public required string ImageUrl { get; set; }
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
}