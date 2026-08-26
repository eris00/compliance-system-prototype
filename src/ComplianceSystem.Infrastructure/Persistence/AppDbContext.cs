using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>,
      IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Case> Cases =>
        Set<Case>();
}