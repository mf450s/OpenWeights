using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weights.Domain.Entities;

namespace Weights.Infrastructure.Data.Configurations;

public class SetHistoryConfiguration : IEntityTypeConfiguration<SetHistory>
{
    public void Configure(EntityTypeBuilder<SetHistory> builder)
    {
        builder.HasKey(sh => sh.Id);

        builder.HasOne(sh => sh.WorkoutSession)
            .WithMany(ws => ws.SetHistories)
            .HasForeignKey(sh => sh.WorkoutSessionId);

        builder.HasOne(sh => sh.Exercise)
            .WithMany(e => e.SetHistories)
            .HasForeignKey(sh => sh.ExerciseId);

        builder.HasOne(sh => sh.WorkoutTemplateExercise)
            .WithMany(wte => wte.SetHistories)
            .HasForeignKey(sh => sh.WorkoutTemplateExerciseId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(sh => sh.SetNumber)
            .IsRequired();

        builder.Property(sh => sh.Weight)
            .HasPrecision(10, 2);

        builder.Property(sh => sh.RIR)
            .HasPrecision(3, 1);

        builder.Property(sh => sh.DistanceMeters)
            .HasPrecision(10, 2);

        builder.Property(sh => sh.Side)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(sh => sh.PerformedAt)
            .IsRequired();

        builder.HasIndex(sh => new { sh.WorkoutSessionId, sh.ExerciseId })
            .HasDatabaseName("IX_SetHistories_WorkoutSessionId_ExerciseId");
    }
}
