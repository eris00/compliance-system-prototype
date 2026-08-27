using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceSystem.Infrastructure.Persistence.Configurations;

public class AuditEntryConfiguration
    : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.ToTable("AuditEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CaseId)
            .IsRequired();

        builder.Property(x => x.ActionType)
            .IsRequired();

        builder.Property(x => x.ActorUserId)
            .IsRequired(false);

        builder.Property(x => x.OccurredAt)
            .IsRequired();

        builder.Property(x => x.OldValue)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.NewValue)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        // AuditEntry belongs to one Case-u.
        builder.HasOne<Case>()
            .WithMany()
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // ActorUserId shows on user that create action
        // Null represents system
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);


        // It's uses when audit timeline are represented
        builder.HasIndex(x => new
        {
            x.CaseId,
            x.OccurredAt
        });
    }
}