using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace RecipeApp.Tests;

public class IngredientHandlers_UnitTests
{
    private RecipeDb CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<RecipeDb>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new RecipeDb(options);
    }

    [Fact]
    public async Task GetAllIngredients_ReturnsAllIngredients()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.AddRange(
            new Ingredient { Name = "Chocolate", Unit = "gram" },
            new Ingredient { Name = "Milk", Unit = "mL" }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await IngredientHandlers.GetAllIngredients(db);

        // Assert
        var okSet = Assert.IsType<Ok<IngredientResponseDTO[]>>(resp);
        var set = okSet.Value;

        Assert.Equal(2, set?.Length);

        Assert.Equal("Chocolate", set?[0].Name);
        Assert.Equal("gram", set?[0].Unit);

        Assert.Equal("Milk", set?[1].Name);
        Assert.Equal("mL", set?[1].Unit);
    }
    
    [Fact]
    public async Task GetIngredient_GetsCorrectIngredient()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.AddRange(
            new Ingredient { Name = "Chocolate", Unit = "gram" },
            new Ingredient { Name = "Milk", Unit = "mL" }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await IngredientHandlers.GetIngredient(1, db);

        // Assert
        var okResp = Assert.IsType<Ok<IngredientResponseDTO>>(resp);
        Assert.Equal(1, okResp.Value!.Id);
        Assert.Equal("Chocolate", okResp.Value!.Name);
        Assert.Equal("gram", okResp.Value!.Unit);
    }
    
    [Fact]
    public async Task GetIngredient_ReturnsNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.AddRange(
            new Ingredient { Name = "Chocolate", Unit = "gram" },
            new Ingredient { Name = "Milk", Unit = "mL" }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await IngredientHandlers.GetIngredient(5, db);

        // Assert
        Assert.IsType<NotFound>(resp);
    }

    [Fact]
    public async Task CreateIngredient_CreatesIngredient()
    {
        // Arrange
        var db = CreateInMemoryDb();

        var request = new IngredientRequestDTO { Name = "Eggs", Unit = "No."};

        // Act
        var resp = await IngredientHandlers.CreateIngredient(request, new IngredientRequestValidator(), db);

        // Assert
        var checkedResp = Assert.IsType<Created<IngredientResponseDTO>>(resp);
        Assert.Equal("/ingredients/1", checkedResp.Location);

        Assert.Equal("Eggs", checkedResp.Value!.Name);
        Assert.Equal("No.", checkedResp.Value!.Unit);

        Assert.Equal(1, await db.Ingredients.CountAsync());
    }

    [Fact]
    public async Task CreateIngredient_ReturnsValidationProblem_WhenNameIsEmpty()
    {
        // Arrange
        var db = CreateInMemoryDb();

        var request = new IngredientRequestDTO { Name = "", Unit = "No."};

        // Act
        var resp = await IngredientHandlers.CreateIngredient(request, new IngredientRequestValidator(), db);

        // Assert
        Assert.IsType<ValidationProblem>(resp);
    }

    [Fact]
    public async Task UpdateIngredient_UpdatesIngredientWhenFound()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.Add(
            new Ingredient { Name = "Flour", Unit = "grams" }
        );
        await db.SaveChangesAsync();

        var request = new IngredientRequestDTO { Name = "Self-Raising Flour", Unit = "milligrams" };

        // Act
        var resp = await IngredientHandlers.UpdateIngredient(1, request, new IngredientRequestValidator(), db);

        // Assert
        Assert.IsType<NoContent>(resp);

        Assert.Equal(1, await db.Ingredients.CountAsync());

        var ingredient = await db.Ingredients.FirstOrDefaultAsync();
        Assert.Equal("Self-Raising Flour", ingredient?.Name);
        Assert.Equal("milligrams", ingredient?.Unit);
    }

    [Fact]
    public async Task UpdateIngredient_ReturnsNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.Add(
            new Ingredient { Name = "Flour", Unit = "grams" }
        );
        await db.SaveChangesAsync();

        var request = new IngredientRequestDTO { Name = "Self-Raising Flour", Unit = "milligrams" };

        // Act
        var resp = await IngredientHandlers.UpdateIngredient(5, request, new IngredientRequestValidator(), db);

        // Assert
        Assert.IsType<NotFound>(resp);
    }

    [Fact]
    public async Task DeleteIngredient_DeletesSuccessfully()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.Add(
            new Ingredient { Name = "Sugar", Unit = "grams" }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await IngredientHandlers.DeleteIngredient(1, db);

        // Assert
        Assert.IsType<NoContent>(resp);

        Assert.Equal(0, await db.Ingredients.CountAsync());
    }
    
    [Fact]
    public async Task DeleteIngredient_ReturnsNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.Add(
            new Ingredient { Name = "Sugar", Unit = "grams" }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await IngredientHandlers.DeleteIngredient(5, db);

        // Assert
        Assert.IsType<NotFound>(resp);
    }
    
    [Fact]
    public async Task DeleteIngredient_DoesNotModifyDatabase_WhenIngredientDoesNotExist()
    {
        // Arrange
        var db = CreateInMemoryDb();
        db.Ingredients.Add(
            new Ingredient { Name = "Sugar", Unit = "grams" }
        );
        await db.SaveChangesAsync();

        // Act
        var resp = await IngredientHandlers.DeleteIngredient(5, db);

        // Assert
        Assert.IsType<NotFound>(resp);

        Assert.Equal(1, await db.Ingredients.CountAsync());

        var ingredient = await db.Ingredients.FirstOrDefaultAsync();
        Assert.Equal("Sugar", ingredient?.Name);
        Assert.Equal("grams", ingredient?.Unit);
    }
}
