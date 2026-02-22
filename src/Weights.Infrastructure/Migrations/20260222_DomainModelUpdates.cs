using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weights.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DomainModelUpdates : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // --- WorkoutTemplateExercise: replace TargetReps with TargetRepsMin/Max + IsAMRAP ---
        migrationBuilder.DropColumn(
            name: "TargetReps",
            table: "WorkoutTemplateExercises");

        migrationBuilder.AddColumn<int>(
            name: "TargetRepsMin",
            table: "WorkoutTemplateExercises",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TargetRepsMax",
            table: "WorkoutTemplateExercises",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsAMRAP",
            table: "WorkoutTemplateExercises",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        // --- SetHistory: add optional FK to WorkoutTemplateExercise ---
        migrationBuilder.AddColumn<int>(
            name: "WorkoutTemplateExerciseId",
            table: "SetHistories",
            type: "integer",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_SetHistories_WorkoutTemplateExerciseId",
            table: "SetHistories",
            column: "WorkoutTemplateExerciseId");

        migrationBuilder.AddForeignKey(
            name: "FK_SetHistories_WorkoutTemplateExercises_WorkoutTemplateExerciseId",
            table: "SetHistories",
            column: "WorkoutTemplateExerciseId",
            principalTable: "WorkoutTemplateExercises",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        // --- SetHistory: composite index (WorkoutSessionId, ExerciseId) ---
        migrationBuilder.CreateIndex(
            name: "IX_SetHistories_WorkoutSessionId_ExerciseId",
            table: "SetHistories",
            columns: new[] { "WorkoutSessionId", "ExerciseId" });

        // --- WorkoutSession: index (UserId, Date) ---
        migrationBuilder.CreateIndex(
            name: "IX_WorkoutSessions_UserId_Date",
            table: "WorkoutSessions",
            columns: new[] { "UserId", "Date" });

        // --- User: add UpdatedAt ---
        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "Users",
            type: "timestamp with time zone",
            nullable: true);

        // --- WorkoutTemplate: add UpdatedAt ---
        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "WorkoutTemplates",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverse WorkoutTemplateExercise changes
        migrationBuilder.DropColumn(name: "TargetRepsMin", table: "WorkoutTemplateExercises");
        migrationBuilder.DropColumn(name: "TargetRepsMax", table: "WorkoutTemplateExercises");
        migrationBuilder.DropColumn(name: "IsAMRAP", table: "WorkoutTemplateExercises");

        migrationBuilder.AddColumn<int>(
            name: "TargetReps",
            table: "WorkoutTemplateExercises",
            type: "integer",
            nullable: true);

        // Reverse SetHistory FK
        migrationBuilder.DropForeignKey(
            name: "FK_SetHistories_WorkoutTemplateExercises_WorkoutTemplateExerciseId",
            table: "SetHistories");
        migrationBuilder.DropIndex(name: "IX_SetHistories_WorkoutTemplateExerciseId", table: "SetHistories");
        migrationBuilder.DropColumn(name: "WorkoutTemplateExerciseId", table: "SetHistories");

        // Reverse indexes
        migrationBuilder.DropIndex(name: "IX_SetHistories_WorkoutSessionId_ExerciseId", table: "SetHistories");
        migrationBuilder.DropIndex(name: "IX_WorkoutSessions_UserId_Date", table: "WorkoutSessions");

        // Reverse UpdatedAt columns
        migrationBuilder.DropColumn(name: "UpdatedAt", table: "Users");
        migrationBuilder.DropColumn(name: "UpdatedAt", table: "WorkoutTemplates");
    }
}
