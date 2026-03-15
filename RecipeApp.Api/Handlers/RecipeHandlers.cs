using Microsoft.EntityFrameworkCore;

public static class RecipeHandlers
{
    public static async Task<IResult> GetRecipeSummaries(RecipeDb db, int skip = 0, int take = 10)
    {
        return TypedResults.Ok(await db.Recipes
            .Skip(skip)
            .Take(take)
            .Include(r => r.Images)
            .Select(r => new RecipeSummaryResponseDTO(r))
            .ToArrayAsync());
    }

    public static async Task<IResult> GetRecipeDetails(int id, RecipeDb db)
    {
        var recipe = await GetFullRecipe(id, db);

        if (recipe is null) return TypedResults.NotFound();

        return TypedResults.Ok(new RecipeDetailsResponseDTO(recipe));
    }

    public static async Task<IResult> CreateRecipe(RecipeRequestDTO recipeRequest, RecipeDb db)
    {
        Recipe recipe = new Recipe
        {
            Name = recipeRequest.Name,
            Description = recipeRequest.Description,
            Servings = recipeRequest.Servings,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RecipeIngredients = recipeRequest.RecipeIngredients
            .Select(ri => new RecipeIngredient { IngredientId = ri.IngredientId, Quantity = ri.Quantity, Notes = ri.Notes })
            .ToList(),
            Steps = recipeRequest.Steps
            .Select(s => new RecipeStep { StepNumber = s.StepNumber, Instructions = s.Instructions, Notes = s.Notes })
            .ToList(),
            Images = recipeRequest.Images.
            Select(i => new RecipeImage { ImageUrl = i.ImageUrl, Caption = i.Caption, IsPrimary = i.IsPrimary })
            .ToList(),
        };
        
        await db.Recipes.AddAsync(recipe);
        await db.SaveChangesAsync();

        var createdRecipe = await GetFullRecipe(recipe.Id, db);

        if(createdRecipe is null) return TypedResults.InternalServerError();

        return TypedResults.Created(
            $"/recipes/{recipe.Id}",
            new RecipeDetailsResponseDTO(createdRecipe));
    }

    private static async Task<Recipe?> GetFullRecipe(int id, RecipeDb db)
    {
        return await db.Recipes
            .Include(r => r.Steps)
            .Include(r => r.Images)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}