# RaceDay — Part 2: RESTful API Development

**Module:** PROG6212 / Programming 2B  
**Project:** RaceDay event management API  
**Stack:** C#, ASP.NET Core Web API (.NET 8), Entity Framework Core, SQL Server, Swagger/OpenAPI, xUnit, GitHub Actions.


## 1. Features implemented

- Registration and login for Organiser and Participant accounts.
- Passwords stored as ASP.NET Core Identity password hashes, never as plaintext.
- Server-side session authentication: `UserId`, `Role`, and `Email` are stored in session; the session cookie is HTTP-only with a 30-minute idle timeout.
- Role checks are enforced on the API endpoints, not only in the UI.
- Profile read/update, event CRUD, route information, event categories, enrolments, and results.
- Ownership checks: an Organiser can manage only their own events/categories and view enrolments/results for their own events; Participants can access only their own enrolments/results.
- Entity Framework Core Code First model and SQL Server provider.
- Swagger UI in Development.
- xUnit integration tests for registration/password hashing, duplicate registration, login success/failure, unauthenticated access, role rejection, and enrolment.
- GitHub Actions workflow restores, builds, and tests the solution on pushes and pull requests.

## 2. Prerequisites

1. Visual Studio 2022 with the **ASP.NET and web development** workload, or the .NET 8 SDK.
2. SQL Server LocalDB (installed with many Visual Studio workloads) or a SQL Server instance.
3. Git and a GitHub account.

## 3. Configure the database

The default connection string targets SQL Server LocalDB. If your machine uses a different SQL Server instance, edit `RaceDay.Api/appsettings.json` or use user secrets/environment configuration. Do not commit passwords or production connection strings.

Open a terminal in the `RaceDay-Part2` folder and run:

```bash
dotnet tool install --global dotnet-ef --version 8.0.11
dotnet restore RaceDay-Part2.sln
dotnet ef migrations add InitialCreate --project RaceDay.Api/RaceDay.Api.csproj --startup-project RaceDay.Api/RaceDay.Api.csproj
dotnet ef database update --project RaceDay.Api/RaceDay.Api.csproj --startup-project RaceDay.Api/RaceDay.Api.csproj
```

This creates an EF Core migration from the Code First entities and applies it to SQL Server. Commit the generated `Migrations` folder. Verify the tables and relationships in SQL Server Object Explorer before recording your demonstration.

## 4. Run the API

```bash
dotnet run --project RaceDay.Api/RaceDay.Api.csproj
```

Use the HTTPS/HTTP URL printed in the terminal. In Development, open `/swagger` to inspect and try the endpoints. Swagger will display the endpoint routes and request schemas. For session-protected calls, register an account, log in, then make subsequent requests from the same browser/client so the session cookie is retained. Use `POST /api/auth/logout` to clear the session.

## 5. Run tests

```bash
dotnet test RaceDay-Part2.sln --configuration Release
```

The tests use EF Core InMemory so they do not require a SQL Server instance. This isolates API behavior tests from local database setup; it does **not** replace testing the API against SQL Server before submission.

## 6. Endpoint summary

| Method | Route | Access | Purpose |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register Participant or Organiser |
| POST | `/api/auth/register-organiser` | Public | Part 1-compatible Organiser registration route |
| POST | `/api/auth/login` | Public | Verify credentials and create session |
| POST | `/api/auth/logout` | Public/session | Clear the current session |
| GET, PUT | `/api/users/me` | Authenticated | View/update own profile |
| GET | `/api/users/{id}` | Organiser | View a user profile |
| GET | `/api/events` | Public | List/filter events |
| GET | `/api/events/{id}` | Public | Event details, categories, routes |
| POST, PUT, DELETE | `/api/events[/{id}]` | Organiser | Create/update/delete own events |
| GET | `/api/events/{id}/routes` | Public | Route information for an event |
| GET, POST | `/api/events/{eventId}/categories` | Public / Organiser | List or add event categories |
| PUT, DELETE | `/api/categories/{id}` | Organiser | Update/delete own event category |
| POST | `/api/enrolments` | Participant | Enrol using a category ID |
| GET | `/api/enrolments/me` | Participant | View own enrolments |
| GET | `/api/events/{eventId}/enrolments` | Organiser | View enrolments for own event |
| DELETE | `/api/enrolments/{id}` | Participant | Cancel own enrolment when no result exists |
| POST | `/api/enrolments/{id}/result` | Organiser | Record an event result |
| PUT | `/api/results/{id}` | Organiser | Update result for own event |
| GET | `/api/results/me` | Participant | View personal results |
| GET | `/api/events/{eventId}/results` | Organiser | View results for own event |
| GET | `/api/results/{id}` | Authenticated | View a result if it belongs to the caller's permitted scope |

Expected failures include `400 Bad Request` for invalid input, `401 Unauthorized` when a protected route is called without a session, `403 Forbidden` for a role/ownership violation, `404 Not Found` for a missing resource, and `409 Conflict` for duplicate email/enrolment, capacity limits, or conflicting deletion/result operations.

## 7. GitHub Actions / CI

The workflow is stored at `workflows/dotnet.yml`. It runs `dotnet restore`, `dotnet build`, and `dotnet test` on every push and pull request.

Before submission:
1. Push this project to your lecturer-provided repository/Part 2 branch.
2. Open the **Actions** tab and confirm the latest run is green.
3. Take a genuine screenshot of the successful workflow run and add it to the repository, for example `docs/ci-green-build.png`.
4. Replace the placeholder below with the screenshot link after you have captured it. Do not use a fabricated screenshot.

**CI screenshot:** `docs/ci-green-build.png` (add after a successful real run).

## 8. Required video presentation

Record your own voice and screen; AI-generated voices are not permitted. Upload the recording as **Unlisted** on YouTube and add the link here before submitting.

Suggested demonstration order:
1. Explain the solution structure and EF Core entities/relationships.
2. Run the migration/database update and show the tables in SQL Server.
3. Launch the API and open Swagger.
4. Register a Participant and an Organiser; show that the stored password is a hash.
5. Log in and demonstrate that the session persists across requests; log out.
6. Demonstrate public event reads, Organiser event/category management, Participant enrolment, and role/ownership failures.
7. Demonstrate result entry and personal result viewing.
8. Run `dotnet test` and show the passing test summary and green GitHub Actions build.

**YouTube video:** `ADD YOUR UNLISTED YOUTUBE URL HERE`

#
