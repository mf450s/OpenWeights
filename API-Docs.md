# Weights API Dokumentation

Vollständige API-Dokumentation mit allen Endpoints, Request/Response-Formaten und Beispielen.

## Base URL

```
http://localhost:8080/api
```

## Authentifizierung

### POST /auth/register

Registriert einen neuen Benutzer.

**Request Body:**
```json
{
  "name": "Tom",
  "email": "tom@example.com",
  "password": "SicheresPasswort123!"
}
```

**Response 201 Created:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Tom",
  "email": "tom@example.com",
  "createdAt": "2024-05-20T10:00:00Z"
}
```

**Response 400 Bad Request:**
```json
{
  "error": "Email already in use"
}
```

### POST /auth/login

Authentifiziert einen Benutzer und gibt JWT-Token zurück.

**Request Body:**
```json
{
  "email": "tom@example.com",
  "password": "SicheresPasswort123!"
}
```

**Response 200 OK:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-05-21T10:00:00Z"
}
```

## Übungen

**Authentifizierung erforderlich** - Füge JWT-Token im Header hinzu:
```
Authorization: Bearer <your-token>
```

### GET /exercises

Ruft alle verfügbaren Übungen ab.

**Query Parameters:**
- `muscleId` (optional): Filter nach Muskelgruppe

**Response 200 OK:**
```json
[
  {
    "id": 1,
    "name": "Barbell Bench Press",
    "trackType": "WeightReps",
    "muscles": [
      {
        "id": 5,
        "name": "Pectoralis Major",
        "targetType": "Primary"
      }
    ]
  }
]
```

### GET /exercises/{id}

Ruft eine einzelne Übung ab.

**Response 200 OK:** Wie oben
**Response 404 Not Found**

## Trainingspläne

### GET /templates

Lädt alle Trainingspläne des eingeloggten Benutzers.

**Response 200 OK:**
```json
[
  {
    "id": 10,
    "name": "Push Day A",
    "description": "Fokus auf Brust und Trizeps",
    "exerciseCount": 5
  }
]
```

### POST /templates

Erstellt einen neuen Trainingsplan.

**Request Body:**
```json
{
  "name": "Leg Day Heavy",
  "description": "Squat Fokus",
  "exercises": [
    {
      "exerciseId": 4,
      "orderIndex": 1,
      "targetSets": 4,
      "targetReps": "5-8",
      "targetRPE": 8.5,
      "restSeconds": 180
    }
  ]
}
```

**Response 201 Created:**
```json
{
  "id": 11,
  "name": "Leg Day Heavy",
  "description": "Squat Fokus",
  "exercises": [
    {
      "exerciseId": 4,
      "exerciseName": "Barbell Squat",
      "orderIndex": 1,
      "targetSets": 4,
      "targetReps": "5-8",
      "targetRPE": 8.5,
      "restSeconds": 180
    }
  ]
}
```

## Workout-Sessions

### POST /sessions

Speichert ein absolviertes Training.

**Request Body:**
```json
{
  "workoutTemplateId": 11,
  "name": "Leg Day Heavy - Montag",
  "date": "2024-05-20",
  "startTime": "2024-05-20T17:00:00Z",
  "endTime": "2024-05-20T18:30:00Z",
  "note": "Knie hat etwas gezwickt",
  "sets": [
    {
      "exerciseId": 4,
      "setNumber": 1,
      "weight": 100.0,
      "reps": 8,
      "rir": 2.0,
      "performedAt": "2024-05-20T17:10:00Z"
    }
  ]
}
```

**Response 201 Created:**
```json
{
  "id": 505,
  "status": "success"
}
```

### GET /sessions/history

Ruft die Historie der Workouts ab.

**Query Parameters:**
- `page` (default: 1)
- `pageSize` (default: 10)

**Response 200 OK:**
```json
{
  "data": [
    {
      "id": 505,
      "name": "Leg Day Heavy - Montag",
      "date": "2024-05-20",
      "totalVolume": 8500.0
    }
  ],
  "totalCount": 45,
  "page": 1
}
```

## Fehler-Responses

Alle Fehler folgen diesem Format:

```json
{
  "error": "Error message"
}
```

**HTTP Status Codes:**
- `200` - OK
- `201` - Created
- `400` - Bad Request (Validierung fehlgeschlagen)
- `401` - Unauthorized (kein/ungültiger Token)
- `404` - Not Found
- `500` - Internal Server Error
