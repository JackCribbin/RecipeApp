using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RecipeDb>(opt => 
    opt.UseSqlite("Data Source=recipes.db"));
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
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

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

app.Run();

public partial class Program { }