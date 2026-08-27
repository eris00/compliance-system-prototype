using ComplianceSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceSystem.Infrastructure.Persistence.Configurations;

public class CaseCategoryConfiguration
    : IEntityTypeConfiguration<CaseCategory>
{
    public void Configure(EntityTypeBuilder<CaseCategory> builder)
    {
        builder.ToTable("CaseCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}