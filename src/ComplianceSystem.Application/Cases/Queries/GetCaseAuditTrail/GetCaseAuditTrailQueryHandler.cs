using ComplianceSystem.Application.Cases.Dtos;
using ComplianceSystem.Application.Common.Exceptions;
using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Application.Common.Security;
using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Cases.Queries.GetCaseAuditTrail;

public class GetCaseAuditTrailQueryHandler
    : IRequestHandler<GetCaseAuditTrailQuery, IReadOnlyList<AuditEntryDto>>
{
    private const string SystemActorName = "System";
    private const string UnknownUserName = "Unknown user";

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetCaseAuditTrailQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<IReadOnlyList<AuditEntryDto>> Handle(
        GetCaseAuditTrailQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CaseId == Guid.Empty)
        {
            throw new DomainException("Case ID is required.");
        }

        var currentUserId = _currentUserService.UserId
            ?? throw new DomainException("Authenticated user is required.");

        var hasGlobalReadScope =
            _currentUserService.IsInRole(AppRoles.Supervisor)
            || _currentUserService.IsInRole(AppRoles.Auditor);

        var isAnalyst = _currentUserService.IsInRole(AppRoles.Analyst);

        if (!hasGlobalReadScope && !isAnalyst)
        {
            throw new DomainException(
                "Only Analysts, Supervisors and Auditors can view case audit trails.");
        }

        var caseQuery = _context.Cases
            .AsNoTracking()
            .Where(x => x.Id == request.CaseId);

        if (!hasGlobalReadScope)
        {
            caseQuery = caseQuery.Where(x => x.AssignedAnalystId == currentUserId);
        }

        var isCaseVisible = await caseQuery.AnyAsync(cancellationToken);

        if (!isCaseVisible)
        {
            throw new NotFoundException("Case was not found.");
        }

        var auditEntries = await _context.AuditEntries
            .AsNoTracking()
            .Where(x => x.CaseId == request.CaseId)
            .OrderBy(x => x.OccurredAt)
            .ThenBy(x => x.Id)
            .Select(x => new AuditEntryProjection(
                x.Id,
                x.CaseId,
                x.ActionType,
                x.ActorUserId,
                x.OccurredAt,
                x.OldValue,
                x.NewValue,
                x.Description))
            .ToListAsync(cancellationToken);

        if (auditEntries.Count == 0)
        {
            return [];
        }

        var userNames = await _identityService.GetUserNamesAsync(
            auditEntries
                .Where(x => x.ActorUserId.HasValue)
                .Select(x => x.ActorUserId!.Value)
                .Distinct(),
            cancellationToken);

        return auditEntries
            .Select(x => new AuditEntryDto(
                x.Id,
                x.CaseId,
                x.ActionType,
                x.ActorUserId,
                GetActorName(userNames, x.ActorUserId),
                x.OccurredAt,
                x.OldValue,
                x.NewValue,
                x.Description))
            .ToList();
    }

    private static string GetActorName(
        IReadOnlyDictionary<Guid, string> userNames,
        Guid? actorUserId)
    {
        if (!actorUserId.HasValue)
        {
            return SystemActorName;
        }

        return userNames.TryGetValue(actorUserId.Value, out var userName)
            ? userName
            : UnknownUserName;
    }

    private sealed record AuditEntryProjection(
        Guid Id,
        Guid CaseId,
        AuditActionType ActionType,
        Guid? ActorUserId,
        DateTime OccurredAt,
        string? OldValue,
        string? NewValue,
        string? Description);
}
