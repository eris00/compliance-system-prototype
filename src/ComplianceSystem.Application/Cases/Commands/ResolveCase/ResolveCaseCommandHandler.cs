using ComplianceSystem.Application.Common.Exceptions;
using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Cases.Commands.ResolveCase;

public class ResolveCaseCommandHandler
    : IRequestHandler<ResolveCaseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ResolveCaseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(
        ResolveCaseCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CaseId == Guid.Empty)
        {
            throw new DomainException("Case ID is required.");
        }

        var currentUserId = _currentUserService.UserId
            ?? throw new DomainException("Authenticated user is required.");

        var complianceCase = await _context.Cases
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.CaseId
                    && x.AssignedAnalystId == currentUserId,
                cancellationToken);

        if (complianceCase is null)
        {
            throw new NotFoundException("Case was not found.");
        }

        complianceCase.Resolve(
            request.Outcome,
            request.Explanation);

        var auditEntry = AuditEntry.Create(
            complianceCase.Id,
            AuditActionType.CaseResolved,
            currentUserId,
            oldValue: CaseStatus.InReview.ToString(),
            newValue: CaseStatus.Resolved.ToString(),
            description:
                $"Case resolved with outcome: {complianceCase.ResolutionOutcome}");

        await _context.AuditEntries.AddAsync(
            auditEntry,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
