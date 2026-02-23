# Weights - Workout Tracking API

Eine moderne Workout-Tracking API entwickelt mit .NET 9 und Entity Framework Core.

## 🎯 Features

- **Benutzer-Authentifizierung** mit JWT Token
- **Workout-Management** (Workouts erstellen, bearbeiten, löschen)
- **Übungs-Datenbank** mit Muskelgruppen-Mapping
- **Workout-Logs** mit verschiedenen Track-Types (Weight+Reps, Bodyweight, Duration, Distance)
- **Mehrsprachigkeit** (Deutsch/Englisch)
- **Swagger/OpenAPI** Dokumentation
- **Docker-Ready** mit Multi-Stage Build

## 🏗️ Architektur

Das Projekt folgt der **Clean Architecture** mit folgenden Layern:

```
src/
├── Weights.API/              # API Layer (Controllers, Middleware)
├── Weights.Application/      # Application Layer (Use Cases, DTOs, Interfaces)
├── Weights.Domain/           # Domain Layer (Entities, Enums, Value Objects)
└── Weights.Infrastructure/   # Infrastructure Layer (Database, Services)
```

## 🚀 Quick Start

### Voraussetzungen

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (oder Docker)
- [Docker](https://www.docker.com/get-started) (optional)

### Mit .NET CLI

1. **Repository klonen**
```bash
git clone https://github.com/mf450s/weights.git
cd weights
```

2. **appsettings.json konfigurieren**
```bash
cd src/Weights.API
cp appsettings.Development.json appsettings.json
```

Passe die Connection String und JWT-Settings an:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=weights;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "SecretKey": "your-super-secret-key-min-32-characters",
    "Issuer": "WeightsAPI",
    "Audience": "WeightsClient",
    "ExpirationMinutes": 60
  }
}
```

3. **Datenbank Migration**
```bash
dotnet ef database update --project ../Weights.Infrastructure --startup-project .
```

4. **API starten**
```bash
dotnet run
```

Die API ist nun unter `https://localhost:5001` verfügbar.
Swagger UI: `https://localhost:5001/swagger`

### Mit Docker

1. **Docker Image bauen**
```bash
docker build -t weights-api .
```

2. **Mit Docker Compose starten** (inkl. PostgreSQL)
```bash
docker-compose up -d
```

Die API läuft auf Port 8080, PostgreSQL auf Port 5432.

## 📚 API Endpoints

### Authentication

#### Registrieren
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

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-02-09T13:30:00Z"
}
```

### Workouts

Alle Workout-Endpoints benötigen einen JWT Token:
```http
Authorization: Bearer <your-token>
```

#### Workout erstellen
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

#### Alle Workouts abrufen
```http
GET /api/workouts
```

#### Workout abrufen
```http
GET /api/workouts/{id}
```

#### Workout aktualisieren
```http
PUT /api/workouts/{id}
Content-Type: application/json

{
  "name": "Updated Push Day",
  "description": "Updated description",
  "exercises": [...]
}
```

#### Workout löschen
```http
DELETE /api/workouts/{id}
```

### Workout Logs

#### Log erstellen (Weight + Reps)
```http
POST /api/workout-logs
Content-Type: application/json

{
  "workoutId": 1,
  "exercises": [
    {
      "workoutExerciseId": 1,
      "sets": [
        {
          "weight": 80,
          "reps": 8,
          "rpe": 8
        },
        {
          "weight": 80,
          "reps": 7,
          "rpe": 9
        }
      ]
    }
  ],
  "notes": "Good session"
}
```

#### Log erstellen (Bodyweight)
```http
POST /api/workout-logs
Content-Type: application/json

{
  "workoutId": 2,
  "exercises": [
    {
      "workoutExerciseId": 4,
      "sets": [
        {
          "reps": 12,
          "rpe": 7
        },
        {
          "reps": 10,
          "rpe": 8
        }
      ]
    }
  ]
}
```

#### Logs abrufen
```http
GET /api/workout-logs?workoutId=1&startDate=2026-01-01&endDate=2026-12-31
```

#### Log löschen
```http
DELETE /api/workout-logs/{id}
```

### Exercises

#### Alle Übungen abrufen
```http
GET /api/exercises
```

#### Übung abrufen
```http
GET /api/exercises/{id}
```

#### Übung erstellen
```http
POST /api/exercises
Content-Type: application/json

{
  "name": "Dumbbell Flyes",
  "description": "Chest isolation exercise",
  "trackType": "WeightReps",
  "targetedMuscles": [
    {
      "muscleId": 1,
      "targetType": "Primary"
    }
  ]
}
```

### Muscles

#### Alle Muskelgruppen abrufen
```http
GET /api/muscles
```

## 🗄️ Datenbank Schema

### Entities

- **User**: Benutzer mit Email, Password Hash
- **Muscle**: Muskelgruppen (Chest, Back, Legs, etc.)
- **Exercise**: Übungen mit TrackType
- **ExerciseMuscle**: Many-to-Many Beziehung zwischen Exercise und Muscle
- **Workout**: Workout-Templates
- **WorkoutExercise**: Übungen in einem Workout
- **WorkoutLog**: Durchgeführte Workouts
- **WorkoutLogExercise**: Übungen im Log
- **WorkoutLogSet**: Einzelne Sets im Log

### TrackTypes

- `WeightReps`: Gewicht + Wiederholungen (z.B. Bench Press)
- `BodyweightReps`: Nur Wiederholungen (z.B. Pull-Ups)
- `Duration`: Zeit in Sekunden (z.B. Plank)
- `Distance`: Distanz in Metern (z.B. Running)

### TargetTypes

- `Primary`: Primär beanspruchter Muskel
- `Secondary`: Sekundär beanspruchter Muskel

## 🔒 Security

- **Password Hashing** mit BCrypt
- **JWT Token** für Authentication
- **Token Expiration** konfigurierbar
- **HTTPS** enforced in Production

## 🌐 Internationalisierung

Die API unterstützt Deutsch (de-DE) und Englisch (en-US).

Sprache über Header setzen:
```http
Accept-Language: de-DE
```

## 🐳 Docker

### Dockerfile

Multi-Stage Build für optimale Image-Größe:
- Build Stage: .NET SDK
- Runtime Stage: .NET ASP.NET Runtime (minimal)

### docker-compose.yml

```yaml
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

## 🧪 Testen

### Mit Swagger UI

1. API starten
2. Browser öffnen: `https://localhost:5001/swagger`
3. "Authorize" klicken und JWT Token eingeben
4. Endpoints testen

### Mit curl

```bash
# Registrieren
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!","username":"testuser"}'

# Login
TOKEN=$(curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!"}' \
  | jq -r '.token')

# Workouts abrufen
curl -X GET https://localhost:5001/api/workouts \
  -H "Authorization: Bearer $TOKEN"
```

## 🔄 CI/CD

### GitHub Actions Workflow

Erstelle `.github/workflows/docker-publish.yml`:

```yaml
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

## 📝 Seed Data

Beim ersten Start werden automatisch Daten eingefügt:

- **12 Muskelgruppen** (Pectoralis, Lats, Quadriceps, etc.)
- **12 Übungen** (Bench Press, Squat, Deadlift, etc.)
- **Muscle-Exercise Mappings**

## 🛠️ Development

### Neue Migration erstellen

```bash
cd src/Weights.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Weights.API
```

### Migration anwenden

```bash
dotnet ef database update --startup-project ../Weights.API
```

### Migration rückgängig machen

```bash
dotnet ef database update PreviousMigration --startup-project ../Weights.API
```

## 📦 NuGet Packages

### API
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore

### Application
- BCrypt.Net-Next
- FluentValidation

### Infrastructure
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.EntityFrameworkCore.Tools

## 🤝 Contributing

1. Branch von `development` erstellen
2. Features implementieren
3. Pull Request erstellen
4. Code Review abwarten
5. Merge in `development`

## 📄 License

MIT License - siehe LICENSE Datei

## 👤 Author

**Tom** - Azubi zum Fachinformatiker für Anwendungsentwicklung bei Trilux

## 🙏 Acknowledgments

- Clean Architecture von Jason Taylor
- Entity Framework Core Team
- ASP.NET Core Team
