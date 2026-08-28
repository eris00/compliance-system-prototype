using ComplianceSystem.Application.Common.Exceptions;
using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Cases.Commands.CloseCase;

public class CloseCaseCommandHandler
    : IRequestHandler<CloseCaseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CloseCaseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(
        CloseCaseCommand request,
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
                x => x.Id == request.CaseId,
                cancellationToken);

        if (complianceCase is null)
        {
            throw new NotFoundException("Case was not found.");
        }

        complianceCase.Close();

        var auditEntry = AuditEntry.Create(
            complianceCase.Id,
            AuditActionType.CaseClosed,
            currentUserId,
            oldValue: CaseStatus.Resolved.ToString(),
            newValue: CaseStatus.Closed.ToString(),
            description: "Case closed.");

        await _context.AuditEntries.AddAsync(
            auditEntry,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
