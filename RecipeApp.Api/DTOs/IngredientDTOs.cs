public class IngredientRequestDTO
{
    public required string Name { get; set; }
    public required string Unit { get; set; }
    public decimal PricePerUnit { get; set; }
}

public class IngredientResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }

    public IngredientResponseDTO(Ingredient ingredient)
    {
        Id = ingredient.Id;
        Name = ingredient.Name;
        Unit = ingredient.Unit;
        PricePerUnit = ingredient.PricePerUnit;
    }
}