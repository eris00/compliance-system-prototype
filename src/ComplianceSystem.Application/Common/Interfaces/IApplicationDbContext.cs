using ComplianceSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Case> Cases { get; }

    DbSet<CaseCategory> CaseCategories { get; }

    DbSet<AuditEntry> AuditEntries { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}