using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RecipeDb>(opt => 
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "RecipeAPI";
    config.Title = "RecipeAPI v1";
    config.Version = "v1";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://recipe-frontend-rho.vercel.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RecipeDb>();
    db.Database.Migrate();
}

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "RecipeAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

var ingredients = app.MapGroup("/ingredients");
ingredients.MapGet("/", IngredientHandlers.GetAllIngredients);
ingredients.MapGet("/{id}", IngredientHandlers.GetIngredient);
ingredients.MapPost("/", IngredientHandlers.CreateIngredient);
ingredients.MapPut("/{id}", IngredientHandlers.UpdateIngredient);
ingredients.MapDelete("/{id}", IngredientHandlers.DeleteIngredient);

var recipes = app.MapGroup("/recipes");
recipes.MapGet("/", RecipeHandlers.GetRecipeSummaries);
recipes.MapGet("/{id}", RecipeHandlers.GetRecipeDetails);
recipes.MapPost("/", RecipeHandlers.CreateRecipe);
recipes.MapPut("/{id}", RecipeHandlers.UpdateRecipe);
recipes.MapDelete("/{id}", RecipeHandlers.DeleteRecipe);

app.Run();

public partial class Program { }