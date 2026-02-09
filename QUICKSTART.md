# 🚀 Quick Start Guide - Weights API

## 5-Minuten Setup mit Docker

### 1. Repository klonen
```bash
git clone https://github.com/mf450s/weights.git
cd weights
```

### 2. Docker Compose starten
```bash
docker-compose up -d
```

Das war's! Die API läuft jetzt auf **http://localhost:8080**

Swagger UI: **http://localhost:8080/swagger**

---

## 🧪 Erste Schritte testen

### 1. Benutzer registrieren

Swagger UI öffnen: http://localhost:8080/swagger

1. Endpoint `/api/auth/register` aufklappen
2. "Try it out" klicken
3. JSON eingeben:
```json
{
  "email": "test@example.com",
  "password": "Test123!",
  "username": "testuser"
}
```
4. "Execute" klicken

### 2. Login und Token erhalten

1. Endpoint `/api/auth/login` aufklappen
2. "Try it out" klicken
3. JSON eingeben:
```json
{
  "email": "test@example.com",
  "password": "Test123!"
}
```
4. "Execute" klicken
5. **Token kopieren** aus der Response

### 3. Token in Swagger einfügen

1. Oben rechts auf "Authorize" 🔒 klicken
2. Token einfügen: `Bearer <dein-token>`
3. "Authorize" klicken
4. "Close" klicken

### 4. Exercises anschauen

1. Endpoint `/api/exercises` (GET) aufklappen
2. "Try it out" klicken
3. "Execute" klicken

Du siehst jetzt alle vorinstallierten Übungen! 🏋️

### 5. Workout erstellen

1. Endpoint `/api/workouts` (POST) aufklappen
2. "Try it out" klicken
3. JSON eingeben:
```json
{
  "name": "Mein erstes Workout",
  "description": "Push Day",
  "exercises": [
    {
      "exerciseId": 1,
      "sets": 4,
      "restTimeSeconds": 90,
      "notes": "Bench Press"
    },
    {
      "exerciseId": 5,
      "sets": 3,
      "restTimeSeconds": 60,
      "notes": "Shoulder Press"
    }
  ]
}
```
4. "Execute" klicken

### 6. Workout Log erstellen

1. Endpoint `/api/workout-logs` (POST) aufklappen
2. "Try it out" klicken
3. JSON eingeben (workoutId muss die ID deines erstellten Workouts sein):
```json
{
  "workoutId": 1,
  "exercises": [
    {
      "workoutExerciseId": 1,
      "sets": [
        {
          "weight": 80,
          "reps": 10,
          "rpe": 7
        },
        {
          "weight": 80,
          "reps": 9,
          "rpe": 8
        },
        {
          "weight": 80,
          "reps": 8,
          "rpe": 9
        }
      ]
    }
  ],
  "notes": "Gutes Training heute!"
}
```
4. "Execute" klicken

---

## 📊 Deine Logs anschauen

1. Endpoint `/api/workout-logs` (GET) aufklappen
2. "Try it out" klicken
3. Optional: `workoutId` eingeben (z.B. 1)
4. "Execute" klicken

Du siehst jetzt alle deine Workout Logs mit allen Details! 💪

---

## 🛠️ Troubleshooting

### Docker Container läuft nicht
```bash
# Logs anschauen
docker-compose logs api

# Container neu starten
docker-compose restart

# Alles neu bauen
docker-compose down
docker-compose up --build -d
```

### Datenbank zurücksetzen
```bash
docker-compose down -v
docker-compose up -d
```

### Port 8080 schon belegt
In `docker-compose.yml` den Port ändern:
```yaml
ports:
  - "9090:8080"  # Statt 8080:8080
```

---

## 📝 Nächste Schritte

- ✅ Du hast erfolgreich:
  - Einen User registriert
  - Dich eingeloggt
  - Ein Workout erstellt
  - Ein Workout Log erstellt
  - Deine Logs abgerufen

- 🚀 Was du noch machen kannst:
  - Eigene Übungen erstellen
  - Verschiedene TrackTypes testen (Bodyweight, Duration, Distance)
  - Workouts aktualisieren
  - Logs filtern nach Datum
  - Muskelgruppen anschauen

- 📚 Mehr Infos:
  - [README.md](README.md) - Vollständige Dokumentation
  - [API Endpoints](README.md#-api-endpoints) - Alle Endpoints im Detail
  - [Database Schema](README.md#%EF%B8%8F-datenbank-schema) - Datenbank Struktur

---

## ❓ Fragen?

Schau in die [README.md](README.md) oder erstelle ein Issue auf GitHub!

Viel Spaß beim Tracken deiner Workouts! 🏋️‍♂️💪
