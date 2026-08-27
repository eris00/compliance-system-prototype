using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceSystem.Infrastructure.Persistence.Configurations;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Severity)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.DueAt)
            .IsRequired();

        builder.Property(x => x.ClosedAt)
            .IsRequired(false);

        // Escalation

        builder.Property(x => x.IsEscalated)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.EscalatedAt)
            .IsRequired(false);

        // Resolution

        builder.Property(x => x.ResolutionOutcome)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(x => x.ResolutionExplanation)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.Property(x => x.ResolvedAt)
            .IsRequired(false);

        // Foreign keys

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired();

        builder.Property(x => x.AssignedAnalystId)
            .IsRequired();

        // Relationships

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.AssignedAnalystId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}