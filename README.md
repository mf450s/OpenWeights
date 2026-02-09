# Weights - Workout Tracking API

🏋️ Backend API für eine Workout Tracking App mit C# (.NET 9), Entity Framework Core und PostgreSQL.

## 📋 Features

- ✅ **Authentifizierung** - JWT-basierte User-Registrierung und Login
- 🏃 **Übungsverwaltung** - Übungen mit Muskelgruppen und Track-Types
- 📝 **Trainingspläne** - Eigene Workout-Templates erstellen und verwalten
- 💪 **Workout-Sessions** - Trainings loggen mit detaillierten Set-Informationen
- 📊 **Historie** - Übersicht über vergangene Workouts mit Volumen-Berechnung
- 🌍 **Lokalisierung** - Unterstützung für DE/EN
- 🐳 **Docker Ready** - Alpine-basiertes Image für Production

## 🏗️ Architektur

Clean Architecture mit klarer Trennung:

```
Weights/
├── src/
│   ├── Weights.Domain/          # Entities, Enums, Interfaces
│   ├── Weights.Application/     # DTOs, Services, Business Logic
│   ├── Weights.Infrastructure/  # EF Core, Repositories, JWT
│   └── Weights.API/            # Controllers, Startup
├── Dockerfile                   # Alpine-based production image
├── docker-compose.yml          # PostgreSQL + API
└── .github/workflows/          # CI/CD Pipeline
```

## 🚀 Quick Start

### Voraussetzungen

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) (optional)
- [PostgreSQL 16](https://www.postgresql.org/download/) (wenn nicht via Docker)

### Option 1: Mit Docker Compose (empfohlen)

```bash
# Repository klonen
git clone https://github.com/mf450s/weights.git
cd weights

# Services starten (PostgreSQL + API)
docker-compose up -d

# API läuft auf http://localhost:8080
# Swagger UI: http://localhost:8080/swagger
```

### Option 2: Lokal mit .NET CLI

```bash
# Repository klonen
git clone https://github.com/mf450s/weights.git
cd weights

# Datenbank-Connection anpassen (appsettings.Development.json)
# Dann:

# Dependencies installieren
dotnet restore

# Datenbank-Migrationen anwenden
dotnet ef database update --project src/Weights.Infrastructure --startup-project src/Weights.API

# API starten
dotnet run --project src/Weights.API

# API läuft auf http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

## 📡 API Endpoints

### Authentifizierung

```http
POST /api/auth/register
POST /api/auth/login
```

### Übungen

```http
GET  /api/exercises              # Alle Übungen
GET  /api/exercises?muscleId=5   # Filter nach Muskelgruppe
GET  /api/exercises/{id}         # Einzelne Übung
```

### Trainingspläne

```http
GET  /api/templates              # Alle Templates des Users
POST /api/templates              # Neues Template erstellen
GET  /api/templates/{id}         # Template mit Übungen
```

### Workout-Sessions

```http
POST /api/sessions               # Workout loggen
GET  /api/sessions/history       # Historie mit Pagination
```

**Alle Endpoints außer Auth benötigen JWT-Token im Authorization Header:**

```
Authorization: Bearer <your-jwt-token>
```

Detaillierte API-Dokumentation in der [Swagger UI](http://localhost:8080/swagger).

## 🗄️ Datenbank

PostgreSQL Schema mit folgenden Tabellen:

- **Users** - Benutzer mit Email/Password
- **Muscles** - Stammdaten Muskelgruppen
- **Exercises** - Übungen mit TrackType
- **Exercise_Muscles** - N:M Verknüpfung
- **Workout_Templates** - Trainingspläne
- **Workout_Template_Exercises** - Übungen im Plan
- **Workout_Sessions** - Absolvierte Trainings
- **Set_History** - Einzelne Sätze mit Gewicht/Reps/RIR

Migrationen werden automatisch beim Start angewendet.

## 🧪 Tests

```bash
# Alle Tests ausführen
dotnet test

# Mit Coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Konfiguration

### Environment Variables

```bash
# Database
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=weights;Username=weights;Password=weights"

# JWT
Jwt__SecretKey="your-secret-key-min-32-chars"
Jwt__Issuer="WeightsAPI"
Jwt__Audience="WeightsClient"
Jwt__ExpirationMinutes="1440"

# ASP.NET
ASPNETCORE_ENVIRONMENT="Production"
ASPNETCORE_URLS="http://+:8080"
```

### appsettings.json

Siehe `src/Weights.API/appsettings.json` für Standard-Konfiguration.

## 🐳 Docker

### Image bauen

```bash
docker build -t weights-api .
```

### Container starten

```bash
docker run -d \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=weights;Username=weights;Password=weights" \
  weights-api
```

### Von GHCR pullen

```bash
docker pull ghcr.io/mf450s/weights:latest
```

## 🔄 CI/CD Pipeline

GitHub Actions Workflow bei Push auf `main` oder `development`:

1. ✅ Restore & Build
2. 🧪 Unit Tests ausführen
3. 🐳 Docker Image bauen (Alpine)
4. 📦 Push zu GitHub Container Registry
5. 💾 Layer Caching (mode=max)

Images verfügbar auf: `ghcr.io/mf450s/weights`

## 📦 Deployment

### Docker Compose (Produktion)

```bash
# example-compose.yml verwenden
cp example-compose.yml docker-compose.prod.yml

# Anpassen und starten
docker-compose -f docker-compose.prod.yml up -d
```

### Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: weights-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: weights-api
  template:
    metadata:
      labels:
        app: weights-api
    spec:
      containers:
      - name: api
        image: ghcr.io/mf450s/weights:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: weights-secrets
              key: db-connection
```

## 🛠️ Entwicklung

### Branch-Strategie

- `main` - Production-ready code
- `development` - Integration branch
- `feature/*` - Feature branches (von development)

### Neue Features

```bash
# Neuen Feature-Branch erstellen
git checkout development
git pull origin development
git checkout -b feature/my-new-feature

# Entwickeln, commiten
git add .
git commit -m "feat: add new feature"

# Push und PR erstellen
git push origin feature/my-new-feature
```

### Code-Stil

- Clean Architecture Prinzipien
- Dependency Injection überall
- Async/Await für alle I/O
- CancellationToken weitergeben
- Fluent Validation

## 📝 License

MIT License - siehe [LICENSE](LICENSE)

## 👤 Autor

**Tom** - [@mf450s](https://github.com/mf450s)

## 🤝 Contributing

Pull Requests sind willkommen! Für größere Änderungen bitte zuerst ein Issue öffnen.
