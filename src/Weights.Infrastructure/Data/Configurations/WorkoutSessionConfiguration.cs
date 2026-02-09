using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weights.Domain.Entities;

namespace Weights.Infrastructure.Data.Configurations;

public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.HasKey(ws => ws.Id);

        builder.Property(ws => ws.Name)
            .HasMaxLength(100);

        builder.Property(ws => ws.Note)
            .HasColumnType("text");

        builder.Property(ws => ws.Date)
            .IsRequired();

        builder.Property(ws => ws.StartTime)
            .IsRequired();

        builder.HasOne(ws => ws.User)
            .WithMany(u => u.WorkoutSessions)
            .HasForeignKey(ws => ws.UserId);

        builder.HasOne(ws => ws.WorkoutTemplate)
            .WithMany(wt => wt.WorkoutSessions)
            .HasForeignKey(ws => ws.WorkoutTemplateId)
            .IsRequired(false);
    }
}
