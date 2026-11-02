# QuestLog

A game catalog REST API built with **ASP.NET Core (.NET 10)** following **Clean Architecture**.
Anyone can browse games, developers, genres and platforms; only admins can create, update or delete them.
Authentication uses **ASP.NET Core Identity** and **JWT bearer tokens**.

## Tech stack

| Area | Technology |
| --- | --- |
| Framework | ASP.NET Core Web API, .NET 10 |
| Data access | Entity Framework Core 10, SQL Server (LocalDB) |
| Auth | ASP.NET Core Identity, JWT (`Microsoft.IdentityModel.JsonWebTokens`), role-based authorization |
| Validation | FluentValidation |
| API docs | OpenAPI + Scalar |

## Architecture

```
QuestLog.Domain           Entities, enums, BaseEntity (no dependencies)
QuestLog.Application      DTOs, validators, services, Result pattern, repository & service interfaces
QuestLog.Infrastructure   EF Core DbContext, configurations, migrations, repositories, Unit of Work, JWT token service
QuestLog.API              Controllers, Program.cs, JWT bearer setup, admin seeder
```

Dependencies point inward: `API → Application ← Infrastructure`, and everything depends on `Domain`.
Interfaces live in Application; Infrastructure provides the implementations.

### Highlights

- **Result pattern:** services return `Result` / `Result<T>` with an `ErrorType` (Validation, NotFound, Conflict, Unauthorized) instead of throwing. A base controller maps these to `400 / 404 / 409 / 401` responses in `ProblemDetails` format.
- **Repository + Unit of Work:** a generic repository with specific repositories per aggregate, all sharing a single `DbContext` through the Unit of Work.
- **Soft delete:** entities carry `IsActive` and `DeletedOn`. Deleted records are hidden by a global query filter, and unique indexes are filtered on active rows only.
- **Sequential GUIDs:** IDs are generated client-side with `Guid.CreateVersion7()`.
- **Slugs:** generated from names and titles (e.g. `The Witcher 3` → `the-witcher-3`). Games are fetched by slug.
- **Paging & filtering:** the games list supports search, genre and platform filters, and pagination (page size capped at 50).
- **Validation:** FluentValidation checks the request format; business rules (duplicate names, missing references) are enforced in services.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (included with Visual Studio). Any SQL Server instance works if you change the connection string.
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/batueren0/QuestLog.git
cd QuestLog
```

### 2. Configure secrets

The JWT signing key and the admin password are **not** stored in the repository. Set them with User Secrets:

```bash
cd QuestLog.API
dotnet user-secrets set "Jwt:SecretKey" "<a random string of at least 32 characters>"
dotnet user-secrets set "Seed:AdminPassword" "<a password that meets the policy below>"
cd ..
```

Password policy (ASP.NET Core Identity defaults + minimum length 8): at least 8 characters, with an uppercase letter, a lowercase letter, a digit and a non-alphanumeric character.

Non-secret settings (issuer, audience, token lifetime, admin email) are in `QuestLog.API/appsettings.json`.

### 3. Create the database

```bash
dotnet ef database update --project QuestLog.Infrastructure --startup-project QuestLog.API
```

### 4. Run

```bash
dotnet run --project QuestLog.API --launch-profile https
```

Open **https://localhost:7081/scalar** for the interactive API reference.

On startup, the app seeds an `Admin` role and an admin user (`admin@questlog.com` with the password from User Secrets). Seeding is idempotent, so restarting does not create duplicates.

## Authentication

1. `POST /api/auth/login` with the admin credentials (or `POST /api/auth/register` to create a regular user).
2. Copy the `token` from the response.
3. Send it on protected requests with this header:

```
Authorization: Bearer <token>
```

| Situation | Response |
| --- | --- |
| No token, invalid token or expired token | `401 Unauthorized` |
| Valid token, but the user is not an admin | `403 Forbidden` |

Tokens are valid for 60 minutes and carry the `sub`, `email`, `name` and `role` claims.

## Endpoints

🔒 = requires the `Admin` role

### Auth

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | Register a new user, returns a token |
| POST | `/api/auth/login` | Log in, returns a token |

### Games

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/games?page=1&pageSize=20&search=&genreId=&platformId=` | Paged list with optional filters |
| GET | `/api/games/{slug}` | Game details (developer, genres, platforms) |
| POST 🔒 | `/api/games` | Create a game |
| PUT 🔒 | `/api/games/{id}` | Full update |
| DELETE 🔒 | `/api/games/{id}` | Soft delete |

### Developers · Genres · Platforms

All three follow the same pattern (`/api/developers`, `/api/genres`, `/api/platforms`):

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/{resource}` | List all |
| GET | `/api/{resource}/{id}` | Get by id |
| POST 🔒 | `/api/{resource}` | Create (`{ "name": "..." }`) |
| PUT 🔒 | `/api/{resource}/{id}` | Update |
| DELETE 🔒 | `/api/{resource}/{id}` | Soft delete (blocked while games still reference it) |

## Example: creating a game

A game needs an existing developer, at least one genre and at least one platform. Create those first, then:

```http
POST /api/games
Authorization: Bearer <admin token>
Content-Type: application/json

{
  "title": "Hollow Knight",
  "description": "A challenging 2D action-adventure through a vast ruined kingdom of insects.",
  "releaseDate": "2017-02-24",
  "coverImageUrl": null,
  "developerId": "<developer id>",
  "genreIds": ["<genre id>"],
  "platformIds": ["<platform id>"]
}
```

Returns `201 Created` with a `Location` header pointing to `/api/games/hollow-knight`.

Errors use the standard `ProblemDetails` format:

```json
{
  "status": 409,
  "title": "Conflict",
  "detail": "A game with this title already exists."
}
```

## Roadmap

- [ ] Reviews: users rate (1–5) and review games (the entity and repository already exist)
- [ ] Library: users track games as Wishlist / Playing / Completed / Dropped, with hours played
- [ ] Slug collision check for developers, genres and platforms
- [ ] Account lockout after repeated failed logins
- [ ] Unit and integration tests

## License

This project was built for learning purposes.
