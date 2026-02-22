using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weights.Domain.Entities;

namespace Weights.Infrastructure.Data.Configurations;

public class WorkoutTemplateExerciseConfiguration : IEntityTypeConfiguration<WorkoutTemplateExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutTemplateExercise> builder)
    {
        builder.HasKey(wte => wte.Id);

        builder.HasOne(wte => wte.WorkoutTemplate)
            .WithMany(wt => wt.WorkoutTemplateExercises)
            .HasForeignKey(wte => wte.WorkoutTemplateId);

        builder.HasOne(wte => wte.Exercise)
            .WithMany(e => e.WorkoutTemplateExercises)
            .HasForeignKey(wte => wte.ExerciseId);

        builder.Property(wte => wte.OrderIndex)
            .IsRequired();

        builder.Property(wte => wte.TargetSets)
            .IsRequired();

        builder.Property(wte => wte.TargetRepsMin)
            .IsRequired(false);

        builder.Property(wte => wte.TargetRepsMax)
            .IsRequired(false);

        builder.Property(wte => wte.IsAMRAP)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(wte => wte.TargetRPE)
            .HasPrecision(3, 1);
    }
}
