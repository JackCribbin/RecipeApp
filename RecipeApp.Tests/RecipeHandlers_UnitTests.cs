using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace RecipeApp.Tests;

public class RecipeHandlers_UnitTests
{
    private RecipeDb CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<RecipeDb>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new RecipeDb(options);
    }

    [Fact]
    public async Task GetAllRecipeSummaries_GetsAllRecipeSummaries()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Recipes.AddRange(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  },
            new Recipe { Name = "Cake", Description = "Chocolate cake", Servings = 8 }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await RecipeHandlers.GetRecipeSummaries(db);

        // Assert
        var okSet = Assert.IsType<Ok<RecipeSummaryResponseDTO[]>>(resp);
        var set = okSet.Value;

        Assert.Equal(2, set?.Length);

        Assert.Equal("Brownies", set?[0].Name);
        Assert.Equal("Fudgy brownies", set?[0].Description);
        Assert.Equal("Brownies.png", set?[0].Image?.ImageUrl);
        Assert.Equal("Fudgy brownies", set?[0].Image?.Caption);
        Assert.True(set?[0].Image?.IsPrimary);

        Assert.Equal("Cake", set?[1].Name);
        Assert.Equal("Chocolate cake", set?[1].Description);
        Assert.Null(set?[1].Image);
    }
    
    [Fact]
    public async Task GetRecipeDetails_ReturnsFullDetails()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.Add(new Ingredient { Name = "Chocolate", Unit = "g" });
        await db.SaveChangesAsync();
        db.Recipes.AddRange(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Steps = [new RecipeStep { Instructions = "Combine all ingredients", Notes = "Don't over-mix", StepNumber = 1}],
            RecipeIngredients = [new RecipeIngredient { IngredientId = 1, Notes = "Use dark chocolate", Quantity = 100 }],
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await RecipeHandlers.GetRecipeDetails(1, db);

        // Assert
        var okSet = Assert.IsType<Ok<RecipeDetailsResponseDTO>>(resp);
        var recipe = okSet.Value;

        Assert.Equal("Brownies", recipe!.Name);
        Assert.Equal("Fudgy brownies", recipe!.Description);
        Assert.Equal(4, recipe?.Servings);

        Assert.Equal("Brownies.png", recipe!.Images.FirstOrDefault()?.ImageUrl);
        Assert.Equal("Fudgy brownies", recipe!.Images.FirstOrDefault()?.Caption);
        Assert.True(recipe?.Images.FirstOrDefault()?.IsPrimary);

        Assert.Equal("Combine all ingredients", recipe!.Steps.FirstOrDefault()?.Instructions);
        Assert.Equal("Don't over-mix", recipe!.Steps.FirstOrDefault()?.Notes);
        Assert.Equal(1, recipe!.Steps.FirstOrDefault()?.StepNumber);

        Assert.Equal("Chocolate", recipe!.RecipeIngredients.FirstOrDefault()?.IngredientName);
        Assert.Equal("g", recipe!.RecipeIngredients.FirstOrDefault()?.IngredientUnit);
        Assert.Equal("Use dark chocolate", recipe!.RecipeIngredients.FirstOrDefault()?.Notes);
        Assert.Equal(100, recipe!.RecipeIngredients.FirstOrDefault()?.Quantity);
    }
    
    [Fact]
    public async Task GetRecipeDetails_ReturnsNotFound_WhenRecipeDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Recipes.AddRange(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  },
            new Recipe { Name = "Cake", Description = "Chocolate cake", Servings = 8 }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await RecipeHandlers.GetRecipeDetails(5, db);

        // Assert
        Assert.IsType<NotFound>(resp);
    }

    [Fact]
    public async Task CreateRecipe_CreatesRecipe()
    {
        // Arrange
        var db = CreateInMemoryDb();

        db.Ingredients.Add(new Ingredient { Name = "Chocolate", Unit = "g" });
        await db.SaveChangesAsync();

        var request = new RecipeRequestDTO
        {
            Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Steps = [new RecipeStepRequestDTO { Instructions = "Combine all ingredients", Notes = "Don't over-mix", StepNumber = 1}],
            RecipeIngredients = [new RecipeIngredientRequestDTO { IngredientId = 1, Notes = "Use dark chocolate", Quantity = 100 }],
            Images = [new RecipeImageRequestDTO { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]
        };

        // Act
        var resp = await RecipeHandlers.CreateRecipe(request, db);

        // Assert
        var checkedResp = Assert.IsType<Created<RecipeDetailsResponseDTO>>(resp);
        Assert.Equal("/recipes/1", checkedResp.Location);

        var returnedRecipe = checkedResp.Value;

        Assert.Equal("Brownies", returnedRecipe!.Name);
        Assert.Equal("Fudgy brownies", returnedRecipe!.Description);
        Assert.Equal(4, returnedRecipe!.Servings);

        var returnedStep = returnedRecipe!.Steps.FirstOrDefault();
        Assert.Equal(1, returnedStep!.StepNumber);
        Assert.Equal("Combine all ingredients", returnedStep!.Instructions);
        Assert.Equal("Don't over-mix", returnedStep!.Notes);
        
        var returnedRecipeIngredient = returnedRecipe!.RecipeIngredients.FirstOrDefault();
        Assert.Equal("Chocolate", returnedRecipeIngredient?.IngredientName);
        Assert.Equal("g", returnedRecipeIngredient?.IngredientUnit);
        Assert.Equal("Use dark chocolate", returnedRecipeIngredient?.Notes);
        Assert.Equal(100, returnedRecipeIngredient?.Quantity);
        
        var returnedRecipeImage = returnedRecipe!.Images.FirstOrDefault();
        Assert.Equal("Brownies.png", returnedRecipeImage?.ImageUrl);
        Assert.Equal("Fudgy brownies", returnedRecipeImage?.Caption);
        Assert.True(returnedRecipeImage?.IsPrimary);

        Assert.Equal(1, await db.Ingredients.CountAsync());
        Assert.Equal(1, await db.Recipes.CountAsync());
    }

    [Fact]
    public async Task UpdateRecipe_UpdatesRecipeWhenFound()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Recipes.Add(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  }
        );
        await db.SaveChangesAsync();

        var request = new RecipeRequestDTO { Name = "Fudgy Brownies", Description = "Fudgy brownies", Servings = 4, 
            Images = [new RecipeImageRequestDTO { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }] };

        // Act
        var resp = await RecipeHandlers.UpdateRecipe(1, request, db);

        // Assert
        Assert.IsType<NoContent>(resp);

        Assert.Equal(1, await db.Recipes.CountAsync());

        var recipe = await db.Recipes.Include(r => r.Images).FirstOrDefaultAsync();
        Assert.Equal("Fudgy Brownies", recipe!.Name);
        Assert.Equal("Fudgy brownies", recipe!.Description);
        Assert.Equal(4, recipe!.Servings);
        Assert.Single(recipe!.Images);
    }

    [Fact]
    public async Task UpdateRecipe_ReturnsNotFound_WhenRecipeDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Recipes.Add(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  }
        );
        await db.SaveChangesAsync();

        var request = new RecipeRequestDTO { Name = "Fudgy Brownies", Description = "Fudgy brownies", Servings = 4, 
            Images = [new RecipeImageRequestDTO { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }] };

        // Act
        var resp = await RecipeHandlers.UpdateRecipe(5, request, db);

        // Assert
        Assert.IsType<NotFound>(resp);
    }

    [Fact]
    public async Task DeleteRecipe_DeletesSuccessfully()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Recipes.Add(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await RecipeHandlers.DeleteRecipe(1, db);

        // Assert
        Assert.IsType<NoContent>(resp);

        Assert.Equal(0, await db.Recipes.CountAsync());
    }
    
    [Fact]
    public async Task DeleteRecipe_ReturnsNotFound_WhenRecipeDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Recipes.Add(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await RecipeHandlers.DeleteRecipe(5, db);

        // Assert
        Assert.IsType<NotFound>(resp);
    }
    
    [Fact]
    public async Task DeleteRecipe_DoesNotModifyDatabase_WhenRecipeDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Recipes.Add(
            new Recipe { Name = "Brownies", Description = "Fudgy brownies", Servings = 4,
            Images = [new RecipeImage { ImageUrl = "Brownies.png", Caption = "Fudgy brownies", IsPrimary = true }]  }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await RecipeHandlers.DeleteRecipe(5, db);

        // Assert
        Assert.IsType<NotFound>(resp);

        Assert.Equal(1, await db.Recipes.CountAsync());

        var recipe = await db.Recipes.FirstOrDefaultAsync();
        Assert.Equal("Brownies", recipe?.Name);
        Assert.Equal("Fudgy brownies", recipe?.Description);
        Assert.Equal(4, recipe?.Servings);
    }
}
