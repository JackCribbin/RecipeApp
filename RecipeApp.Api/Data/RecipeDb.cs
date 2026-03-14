using Microsoft.EntityFrameworkCore;

public class RecipeDb : DbContext
{
    public RecipeDb(DbContextOptions<RecipeDb> options) : base(options) { }

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();
    public DbSet<RecipeImage> RecipeImages => Set<RecipeImage>();
}