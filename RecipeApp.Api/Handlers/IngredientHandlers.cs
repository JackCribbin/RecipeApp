using FluentValidation;
using Microsoft.EntityFrameworkCore;

public static class IngredientHandlers
{
    public static async Task<IResult> GetAllIngredients(RecipeDb db)
    {
        return TypedResults.Ok(
            await db.Ingredients
                .Select(i => new IngredientResponseDTO(i))
                .ToArrayAsync()
        );
    }

    public static async Task<IResult> GetIngredient(int id, RecipeDb db)
    {
        return await db.Ingredients.FindAsync(id)
            is Ingredient ingredient
                ? TypedResults.Ok(new IngredientResponseDTO(ingredient))
                : TypedResults.NotFound();
    }

    public static async Task<IResult> CreateIngredient(
        IngredientRequestDTO dto, IValidator<IngredientRequestDTO> validator, RecipeDb db)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        
        var ingredient = new Ingredient
        {
            Name = dto.Name,
            Unit = dto.Unit,
            PricePerUnit = dto.PricePerUnit
        };

        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync();

        return TypedResults.Created(
            $"/ingredients/{ingredient.Id}",
            new IngredientResponseDTO(ingredient)
        );
    }

    public static async Task<IResult> UpdateIngredient(
        int id, IngredientRequestDTO dto, IValidator<IngredientRequestDTO> validator, RecipeDb db)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var ingredient = await db.Ingredients.FindAsync(id);
        if (ingredient is null) return TypedResults.NotFound();

        ingredient.Name = dto.Name;
        ingredient.Unit = dto.Unit;
        ingredient.PricePerUnit = dto.PricePerUnit;

        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static async Task<IResult> DeleteIngredient(int id, RecipeDb db)
    {
        if (await db.Ingredients.FindAsync(id) is Ingredient ingredient)
        {
            db.Ingredients.Remove(ingredient);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        return TypedResults.NotFound();
    }
}