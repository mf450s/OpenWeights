# Weights — Workout Tracking API

A modern, clean-architecture workout tracking REST API built with .NET 9 and Entity Framework Core.

---

## ✨ Features

- **JWT Authentication** — Secure registration and login flow
- **Workout Management** — Full CRUD for workout templates
- **Exercise Database** — Pre-seeded exercises with muscle group mappings
- **Workout Logs** — Track sets with multiple tracking modes (Weight & Reps, Bodyweight, Duration, Distance)
- **Internationalization** — English and German support via `Accept-Language` header
- **OpenAPI / Swagger UI** — Interactive API documentation out of the box
- **Docker Ready** — Multi-stage Dockerfile and Docker Compose included

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles, strictly separating concerns across four layers:

```
src/
├── Weights.API/              # Controllers, middleware, program entry point
├── Weights.Application/      # Use cases, DTOs, interfaces, validation
├── Weights.Domain/           # Entities, enums, value objects (no dependencies)
└── Weights.Infrastructure/   # EF Core, PostgreSQL, service implementations
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) — or use Docker Compose to spin one up automatically
- [Docker](https://www.docker.com/get-started) *(Option B only)*
- [`jq`](https://stedolan.github.io/jq/) *(required for the curl examples — `brew install jq` / `apt install jq`)*

---

### Option A — .NET CLI

**1. Clone the repository**
```bash
git clone https://github.com/mf450s/weights.git
cd weights
```

**2. Configure application settings**
```bash
cd src/Weights.API
cp appsettings.Development.json appsettings.json
```

Edit `appsettings.json` with your database credentials and JWT secret:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=weights;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "SecretKey": "your-super-secret-key-minimum-32-characters",
    "Issuer": "WeightsAPI",
    "Audience": "WeightsClient",
    "ExpirationMinutes": 60
  }
}
```

> ⚠️ The JWT `SecretKey` must be at least 32 characters long.

**3. Apply database migrations**
```bash
dotnet ef database update --project ../Weights.Infrastructure --startup-project .
```

**4. Run the API**
```bash
dotnet run
```

The API is now available at `https://localhost:5001`.  
Swagger UI: `https://localhost:5001/swagger`

---

### Option B — Docker Compose

Starts both the API and a PostgreSQL instance. After the containers are up, you must run the database migrations once before the API will accept requests.

**1. Start the containers**
```bash
docker-compose up -d
```

**2. Run migrations**
```bash
docker-compose exec api dotnet ef database update \
  --project Weights.Infrastructure \
  --startup-project Weights.API
```

> ℹ️ The API container will restart and fail until migrations have been applied. This is expected on first run.

| Service    | Address               |
|------------|-----------------------|
| API        | `http://localhost:8080` |
| PostgreSQL | `localhost:5432`      |

```yaml
# docker-compose.yml
services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=db;Database=weights;Username=postgres;Password=weights123
    depends_on:
      - db

  db:
    image: postgres:16-alpine
    environment:
      - POSTGRES_DB=weights
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=weights123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

---

## ⚙️ Environment Variables

All configuration can be passed as environment variables, which takes precedence over `appsettings.json`. This is the recommended approach for Docker and cloud deployments.

| Variable                                  | Description                                      | Default (dev)         |
|-------------------------------------------|--------------------------------------------------|-----------------------|
| `ConnectionStrings__DefaultConnection`    | PostgreSQL connection string                     | *(required)*          |
| `Jwt__SecretKey`                          | Signing key for JWT tokens (min. 32 characters)  | *(required)*          |
| `Jwt__Issuer`                             | JWT issuer claim                                 | `WeightsAPI`          |
| `Jwt__Audience`                           | JWT audience claim                               | `WeightsClient`       |
| `Jwt__ExpirationMinutes`                  | Token lifetime in minutes                        | `60`                  |
| `ASPNETCORE_ENVIRONMENT`                  | Runtime environment (`Development`/`Production`) | `Production`          |

> ⚠️ Never commit real secrets to source control. Use environment variables or a secrets manager in production.

---

### Authentication

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "username": "johndoe"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```
```json
// Response
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-02-09T13:30:00Z"
}
```

---

### Workouts

All workout endpoints require a valid JWT token:
```http
Authorization: Bearer <your-token>
```

| Method   | Endpoint             | Description               |
|----------|----------------------|---------------------------|
| `POST`   | `/api/workouts`      | Create a workout template |
| `GET`    | `/api/workouts`      | List all workouts         |
| `GET`    | `/api/workouts/{id}` | Get a single workout      |
| `PUT`    | `/api/workouts/{id}` | Update a workout          |
| `DELETE` | `/api/workouts/{id}` | Delete a workout          |

**Example — Create Workout**
```http
POST /api/workouts
Content-Type: application/json

{
  "name": "Push Day",
  "description": "Chest, Shoulders, Triceps",
  "exercises": [
    {
      "exerciseId": 1,
      "sets": 4,
      "restTimeSeconds": 90,
      "notes": "Focus on form"
    },
    {
      "exerciseId": 5,
      "sets": 3,
      "restTimeSeconds": 60
    }
  ]
}
```

---

### Workout Logs

| Method   | Endpoint                                                   | Description        |
|----------|------------------------------------------------------------|--------------------|
| `POST`   | `/api/workout-logs`                                        | Log a session      |
| `GET`    | `/api/workout-logs?workoutId=1&startDate=...&endDate=...`  | Query logs         |
| `DELETE` | `/api/workout-logs/{id}`                                   | Delete a log entry |

**Example — Log Weight & Reps Session**
```http
POST /api/workout-logs
Content-Type: application/json

{
  "workoutId": 1,
  "notes": "Good session",
  "exercises": [
    {
      "workoutExerciseId": 1,
      "sets": [
        { "weight": 80, "reps": 8, "rpe": 8 },
        { "weight": 80, "reps": 7, "rpe": 9 }
      ]
    }
  ]
}
```

**Example — Log Bodyweight Session**
```http
POST /api/workout-logs
Content-Type: application/json

{
  "workoutId": 2,
  "exercises": [
    {
      "workoutExerciseId": 4,
      "sets": [
        { "reps": 12, "rpe": 7 },
        { "reps": 10, "rpe": 8 }
      ]
    }
  ]
}
```

---

### Exercises

| Method | Endpoint             | Description          |
|--------|----------------------|----------------------|
| `GET`  | `/api/exercises`     | List all exercises   |
| `GET`  | `/api/exercises/{id}`| Get a single exercise|
| `POST` | `/api/exercises`     | Create an exercise   |

**Example — Create Exercise**
```http
POST /api/exercises
Content-Type: application/json

{
  "name": "Dumbbell Flyes",
  "description": "Chest isolation exercise",
  "trackType": "WeightReps",
  "targetedMuscles": [
    { "muscleId": 1, "targetType": "Primary" }
  ]
}
```

### Muscles

```http
GET /api/muscles    # Returns all muscle groups
```

---

## 🗄️ Data Model

### Entities

| Entity               | Description                                        |
|----------------------|----------------------------------------------------|
| `User`               | Authenticated user with hashed password            |
| `Muscle`             | Muscle group (e.g. Pectoralis, Quadriceps)         |
| `Exercise`           | Exercise definition with a TrackType               |
| `ExerciseMuscle`     | Many-to-many: Exercise ↔ Muscle                    |
| `Workout`            | Reusable workout template                          |
| `WorkoutExercise`    | Exercise entry within a workout template           |
| `WorkoutLog`         | Record of a completed workout session              |
| `WorkoutLogExercise` | Exercise entry within a log                        |
| `WorkoutLogSet`      | Individual set data within a log exercise          |

### Track Types

| Value           | Use Case                       | Tracked Fields        |
|-----------------|--------------------------------|-----------------------|
| `WeightReps`    | Barbell / dumbbell exercises   | Weight (kg) + Reps    |
| `BodyweightReps`| Calisthenics (e.g. Pull-Ups)   | Reps only             |
| `Duration`      | Holds (e.g. Plank)             | Time in seconds       |
| `Distance`      | Cardio (e.g. Running)          | Distance in meters    |

### Target Types

| Value       | Meaning                              |
|-------------|--------------------------------------|
| `Primary`   | Muscle group primarily trained       |
| `Secondary` | Muscle group secondarily involved    |

---

## 🔒 Security

- Passwords hashed with **BCrypt**
- Stateless authentication via **JWT Bearer tokens**
- Configurable token expiration
- HTTPS enforced in production

---

## 🌐 Internationalization

The API supports **English** (`en-US`) and **German** (`de-DE`).

Set the language via request header:
```http
Accept-Language: de-DE
```

---

## 🧪 Testing

### Swagger UI

1. Start the API
2. Open `https://localhost:5001/swagger`
3. Click the **Authorize** button (🔒) in the top right
4. In the value field, enter your token **without** the `Bearer ` prefix — Swagger adds it automatically
5. Explore and test all endpoints interactively

### curl

```bash
# Register
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!","username":"testuser"}'

# Login and capture token
TOKEN=$(curl -s -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!"}' \
  | jq -r '.token')

# Fetch workouts
curl -X GET https://localhost:5001/api/workouts \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🌱 Seed Data

On first startup, the following data is automatically seeded:

- **12 muscle groups** — Pectoralis, Latissimus, Quadriceps, and more
- **12 exercises** — Bench Press, Squat, Deadlift, and more
- **Muscle–Exercise mappings** with Primary/Secondary target types

---

## 🛠️ Development

### Migrations

```bash
# Create a new migration
cd src/Weights.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../Weights.API

# Apply pending migrations
dotnet ef database update --startup-project ../Weights.API

# Roll back to a previous migration
dotnet ef database update <PreviousMigrationName> --startup-project ../Weights.API
```

---

## 📦 Key Dependencies

| Layer          | Package                                          |
|----------------|--------------------------------------------------|
| API            | `Microsoft.AspNetCore.Authentication.JwtBearer`  |
| API            | `Swashbuckle.AspNetCore`                         |
| Application    | `FluentValidation`                               |
| Application    | `BCrypt.Net-Next`                                |
| Infrastructure | `Microsoft.EntityFrameworkCore`                  |
| Infrastructure | `Npgsql.EntityFrameworkCore.PostgreSQL`          |
| Infrastructure | `Microsoft.EntityFrameworkCore.Tools`            |

---

## 🔄 CI/CD

The included GitHub Actions workflow triggers on every push to `main` or `development`, and on pull requests targeting `main`. It builds the Docker image and pushes it to **GitHub Container Registry (GHCR)** under two tags: `latest` and the full commit SHA for traceability.

To pull the published image:
```bash
docker pull ghcr.io/<your-github-username>/weights:latest
```

No additional secrets need to be configured — the workflow uses the built-in `GITHUB_TOKEN`.

```yaml
# .github/workflows/docker-publish.yml
name: Docker Build and Push

on:
  push:
    branches: [ main, development ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Login to GitHub Container Registry
        uses: docker/login-action@v2
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build and push
        uses: docker/build-push-action@v4
        with:
          context: .
          push: true
          tags: |
            ghcr.io/${{ github.repository }}:latest
            ghcr.io/${{ github.repository }}:${{ github.sha }}
```

---

## ⚠️ Known Limitations

- **No refresh tokens** — JWT tokens cannot be refreshed; users must re-authenticate after expiry
- **No pagination** — list endpoints (`GET /api/workouts`, `GET /api/exercises`, etc.) return all records without limit or offset support
- **No role-based access** — all authenticated users share the same permission level; there is no admin role
- **No rate limiting** — the API does not currently throttle requests

---

## 🗺️ Roadmap

- [ ] Refresh token support
- [ ] Pagination for list endpoints
- [ ] User profile endpoint (update username, change password)
- [ ] Progress analytics endpoint (volume over time per exercise)
- [ ] Role-based access control for exercise/muscle administration

---



Contributions, bug reports, and feature requests are welcome. Please open an issue before starting work on a significant change so we can discuss the approach first.

**Branch naming**

| Type    | Pattern                     | Example                        |
|---------|-----------------------------|--------------------------------|
| Feature | `feature/<short-description>` | `feature/refresh-token`      |
| Fix     | `fix/<short-description>`     | `fix/log-delete-auth`        |
| Chore   | `chore/<short-description>`   | `chore/update-dependencies`  |

**Workflow**

1. Open an issue describing the bug or feature
2. Fork the repository and branch off from `development`
3. Implement your changes with clear, atomic commits
4. Ensure the project builds and all existing functionality works
5. Open a Pull Request against `development` with a description referencing the issue
6. Wait for code review and address any feedback
7. A maintainer will merge after approval

---

## 📄 License

Distributed under the **MIT License**. See `LICENSE` for details.

---

## 👤 Author

**Tom** — Apprentice Software Developer at Trilux

---

## 🙏 Acknowledgements

- Inspired by [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture) by Jason Taylor
- [Entity Framework Core](https://github.com/dotnet/efcore) team
- [ASP.NET Core](https://github.com/dotnet/aspnetcore) team