using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Cases.Commands.EscalateOverdueCases;

public class EscalateOverdueCasesCommandHandler
    : IRequestHandler<EscalateOverdueCasesCommand, int>
{
    private readonly IApplicationDbContext _context;

    public EscalateOverdueCasesCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(
        EscalateOverdueCasesCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var overdueCases = await _context.Cases
            .Where(x =>
                !x.IsEscalated
                && x.DueAt <= utcNow
                && (x.Status == CaseStatus.Open
                    || x.Status == CaseStatus.InReview))
            .ToListAsync(cancellationToken);

        if (overdueCases.Count == 0)
        {
            return 0;
        }

        foreach (var complianceCase in overdueCases)
        {
            complianceCase.Escalate(utcNow);

            var auditEntry = AuditEntry.Create(
                complianceCase.Id,
                AuditActionType.CaseEscalated,
                actorUserId: null,
                oldValue: bool.FalseString,
                newValue: bool.TrueString,
                description:
                    "Case automatically escalated after its due date.");

            await _context.AuditEntries.AddAsync(
                auditEntry,
                cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return overdueCases.Count;
    }
}
