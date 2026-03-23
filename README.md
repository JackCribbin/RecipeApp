# RecipeApp API

A RESTful API for managing recipes, built with ASP.NET Core Minimal APIs and Entity Framework Core. This is a personal project I built to learn .NET backend development and modern API design patterns.

**Live API:** https://recipeapp-api.azurewebsites.net/swagger

---

## Tech Stack

- **ASP.NET Core 10** — Minimal APIs for lightweight, fast endpoint definition
- **Entity Framework Core** — ORM for database access with code-first migrations
- **SQLite** — Lightweight relational database, suitable for a portfolio project
- **FluentValidation** — Request validation with expressive, testable rule definitions
- **NSwag** — Auto-generated Swagger/OpenAPI documentation
- **xUnit** — Unit testing framework
- **Azure App Service** — Cloud hosting via GitHub Actions CI/CD pipeline

---

## Features

- Full CRUD for recipes and ingredients
- Recipes support nested steps, ingredients and images in a single request
- Shared global ingredient list — ingredients are defined once and reused across recipes
- Pagination on recipe list endpoint with configurable skip/take parameters
- Input validation on all write endpoints with meaningful error messages
- Automatic database migrations on startup
- Swagger UI for interactive API testing

---

## Architecture Decisions

**Minimal APIs over Controllers**
I used ASP.NET Core Minimal APIs rather than the traditional controller-based approach. This keeps the code concise and co-located — route registration and handler logic are easy to follow without the ceremony of controller classes.

**Handler classes for testability**
Handler functions are defined as static methods on dedicated handler classes (`IngredientHandlers`, `RecipeHandlers`) rather than inline lambdas. This makes them independently testable without needing to spin up the full HTTP pipeline.

**Shared ingredient model**
Ingredients exist as a global list that recipes reference via a join table (`RecipeIngredient`). This enables future features like "find recipes I can make from my fridge" by querying which recipes have all their ingredients available.

**DTO separation**
Request and response DTOs are kept separate from EF Core model classes. This prevents circular reference issues during serialisation, controls exactly what data is exposed to clients, and means database schema changes don't automatically break the API contract.

**FluentValidation with cross-collection rules**
Validation includes cross-collection rules that plain data annotations can't express — for example, ensuring recipe step numbers are continuous (1, 2, 3 not 1, 2, 6) and that a recipe has at most one primary image.

---

## Project Structure

```
RecipeApp/
├── RecipeApp.Api/
│   ├── Data/               # DbContext
│   ├── DTOs/               # Request and response DTOs
│   ├── Handlers/           # Endpoint handler functions
│   ├── Models/             # EF Core entity classes
│   ├── Validators/         # FluentValidation validators
│   ├── Migrations/         # EF Core database migrations
│   └── Program.cs          # App configuration and route registration
└── RecipeApp.Tests/        # xUnit unit tests
```

---

## Running Locally

**Prerequisites**
- .NET 10 SDK
- Git

**Steps**

1. Clone the repository
```
git clone https://github.com/JackCribbin/RecipeApp.git
cd RecipeApp
```

2. Run the API
```
cd RecipeApp.Api
dotnet run
```

3. Open Swagger UI at `http://localhost:[port]/swagger`

The database is created and migrations are applied automatically on first run. No additional setup is required.

---

## API Endpoints

Full interactive documentation is available at the [Swagger UI](https://recipeapp-api.azurewebsites.net/swagger).

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/ingredients` | Get all ingredients |
| GET | `/ingredients/{id}` | Get ingredient by ID |
| POST | `/ingredients` | Create ingredient |
| PUT | `/ingredients/{id}` | Update ingredient |
| DELETE | `/ingredients/{id}` | Delete ingredient |
| GET | `/recipes` | Get recipe summaries (paginated) |
| GET | `/recipes/{id}` | Get full recipe detail |
| POST | `/recipes` | Create recipe |
| PUT | `/recipes/{id}` | Update recipe |
| DELETE | `/recipes/{id}` | Delete recipe |

---

## Testing

```
cd RecipeApp
dotnet test
```

Tests cover happy paths, sad paths, validation failures and database side effects for all handlers.

---

## Deployment

The API is deployed to Azure App Service via a GitHub Actions CI/CD pipeline. Every push to `main` triggers an automatic build and deployment. See `.github/workflows/deploy.yml` for the workflow configuration.