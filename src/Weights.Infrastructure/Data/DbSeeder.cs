using Weights.Domain.Entities;
using Weights.Domain.Enums;

namespace Weights.Infrastructure.Data;

public static class DbSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        // Check if data already exists
        if (context.Muscles.Any())
            return;

        // Seed Muscles
        var muscles = new List<Muscle>
        {
            new() { Name = "Pectoralis Major", BodyPart = "Chest" },
            new() { Name = "Anterior Deltoid", BodyPart = "Shoulders" },
            new() { Name = "Triceps Brachii", BodyPart = "Arms" },
            new() { Name = "Latissimus Dorsi", BodyPart = "Back" },
            new() { Name = "Trapezius", BodyPart = "Back" },
            new() { Name = "Biceps Brachii", BodyPart = "Arms" },
            new() { Name = "Quadriceps", BodyPart = "Legs" },
            new() { Name = "Hamstrings", BodyPart = "Legs" },
            new() { Name = "Glutes", BodyPart = "Legs" },
            new() { Name = "Calves", BodyPart = "Legs" },
            new() { Name = "Rectus Abdominis", BodyPart = "Core" },
            new() { Name = "Obliques", BodyPart = "Core" }
        };

        context.Muscles.AddRange(muscles);
        context.SaveChanges();

        // Seed Exercises
        var exercises = new List<Exercise>
        {
            new() { Name = "Barbell Bench Press", TrackType = TrackType.WeightReps, Description = "Compound chest exercise" },
            new() { Name = "Barbell Squat", TrackType = TrackType.WeightReps, Description = "Compound leg exercise" },
            new() { Name = "Barbell Deadlift", TrackType = TrackType.WeightReps, Description = "Full body compound" },
            new() { Name = "Pull Up", TrackType = TrackType.BodyweightReps, Description = "Bodyweight back exercise" },
            new() { Name = "Dumbbell Shoulder Press", TrackType = TrackType.WeightReps, Description = "Shoulder compound" },
            new() { Name = "Barbell Row", TrackType = TrackType.WeightReps, Description = "Back compound" },
            new() { Name = "Leg Press", TrackType = TrackType.WeightReps, Description = "Leg machine exercise" },
            new() { Name = "Dumbbell Bicep Curl", TrackType = TrackType.WeightReps, Description = "Bicep isolation" },
            new() { Name = "Tricep Dips", TrackType = TrackType.BodyweightReps, Description = "Bodyweight triceps" },
            new() { Name = "Plank", TrackType = TrackType.Duration, Description = "Core stability" },
            new() { Name = "Running", TrackType = TrackType.Distance, Description = "Cardio" },
            new() { Name = "Incline Dumbbell Press", TrackType = TrackType.WeightReps, Description = "Upper chest focus" }
        };

        context.Exercises.AddRange(exercises);
        context.SaveChanges();

        // Seed Exercise-Muscle relationships
        var exerciseMuscles = new List<ExerciseMuscle>
        {
            // Barbell Bench Press
            new() { ExerciseId = 1, MuscleId = 1, TargetType = TargetType.Primary },    // Pectoralis
            new() { ExerciseId = 1, MuscleId = 2, TargetType = TargetType.Secondary },  // Anterior Deltoid
            new() { ExerciseId = 1, MuscleId = 3, TargetType = TargetType.Secondary },  // Triceps

            // Barbell Squat
            new() { ExerciseId = 2, MuscleId = 7, TargetType = TargetType.Primary },    // Quadriceps
            new() { ExerciseId = 2, MuscleId = 8, TargetType = TargetType.Secondary },  // Hamstrings
            new() { ExerciseId = 2, MuscleId = 9, TargetType = TargetType.Secondary },  // Glutes

            // Barbell Deadlift
            new() { ExerciseId = 3, MuscleId = 8, TargetType = TargetType.Primary },    // Hamstrings
            new() { ExerciseId = 3, MuscleId = 4, TargetType = TargetType.Primary },    // Lats
            new() { ExerciseId = 3, MuscleId = 9, TargetType = TargetType.Secondary },  // Glutes

            // Pull Up
            new() { ExerciseId = 4, MuscleId = 4, TargetType = TargetType.Primary },    // Lats
            new() { ExerciseId = 4, MuscleId = 6, TargetType = TargetType.Secondary },  // Biceps

            // Dumbbell Shoulder Press
            new() { ExerciseId = 5, MuscleId = 2, TargetType = TargetType.Primary },    // Anterior Deltoid
            new() { ExerciseId = 5, MuscleId = 3, TargetType = TargetType.Secondary },  // Triceps

            // Barbell Row
            new() { ExerciseId = 6, MuscleId = 4, TargetType = TargetType.Primary },    // Lats
            new() { ExerciseId = 6, MuscleId = 5, TargetType = TargetType.Secondary },  // Traps

            // Leg Press
            new() { ExerciseId = 7, MuscleId = 7, TargetType = TargetType.Primary },    // Quadriceps
            new() { ExerciseId = 7, MuscleId = 9, TargetType = TargetType.Secondary },  // Glutes

            // Dumbbell Bicep Curl
            new() { ExerciseId = 8, MuscleId = 6, TargetType = TargetType.Primary },    // Biceps

            // Tricep Dips
            new() { ExerciseId = 9, MuscleId = 3, TargetType = TargetType.Primary },    // Triceps
            new() { ExerciseId = 9, MuscleId = 1, TargetType = TargetType.Secondary },  // Chest

            // Plank
            new() { ExerciseId = 10, MuscleId = 11, TargetType = TargetType.Primary },  // Rectus Abdominis
            new() { ExerciseId = 10, MuscleId = 12, TargetType = TargetType.Secondary },// Obliques

            // Incline Dumbbell Press
            new() { ExerciseId = 12, MuscleId = 1, TargetType = TargetType.Primary },   // Pectoralis
            new() { ExerciseId = 12, MuscleId = 2, TargetType = TargetType.Secondary }, // Anterior Deltoid
            new() { ExerciseId = 12, MuscleId = 3, TargetType = TargetType.Secondary }  // Triceps
        };

        context.ExerciseMuscles.AddRange(exerciseMuscles);
        context.SaveChanges();
    }
}
