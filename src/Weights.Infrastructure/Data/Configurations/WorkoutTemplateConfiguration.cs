using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weights.Domain.Entities;

namespace Weights.Infrastructure.Data.Configurations;

public class WorkoutTemplateConfiguration : IEntityTypeConfiguration<WorkoutTemplate>
{
    public void Configure(EntityTypeBuilder<WorkoutTemplate> builder)
    {
        builder.HasKey(wt => wt.Id);

        builder.Property(wt => wt.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(wt => wt.Description)
            .HasMaxLength(500);

        builder.Property(wt => wt.IsArchived)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(wt => wt.UpdatedAt)
            .IsRequired(false);

        builder.HasOne(wt => wt.User)
            .WithMany(u => u.WorkoutTemplates)
            .HasForeignKey(wt => wt.UserId);
    }
}
