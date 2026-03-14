public class Ingredient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Unit { get; set; }
    public decimal PricePerUnit { get; set; }
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}