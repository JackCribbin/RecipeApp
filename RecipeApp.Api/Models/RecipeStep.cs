public class RecipeStep
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int StepNumber { get; set; }
    public required string Instructions { get; set; }
    public string? Notes { get; set; }
}