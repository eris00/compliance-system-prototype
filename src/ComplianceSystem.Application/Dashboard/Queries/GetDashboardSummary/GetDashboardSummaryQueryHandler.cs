using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Application.Common.Security;
using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private const string UnknownUserName = "Unknown user";

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetDashboardSummaryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new DomainException("Authenticated user is required.");

        var hasGlobalReadScope =
            _currentUserService.IsInRole(AppRoles.Supervisor)
            || _currentUserService.IsInRole(AppRoles.Auditor);

        var isAnalyst = _currentUserService.IsInRole(AppRoles.Analyst);

        if (!hasGlobalReadScope && !isAnalyst)
        {
            throw new DomainException(
                "Only Analysts, Supervisors and Auditors can view the dashboard.");
        }

        var casesQuery = _context.Cases
            .AsNoTracking();

        if (!hasGlobalReadScope)
        {
            casesQuery = casesQuery.Where(x => x.AssignedAnalystId == currentUserId);
        }

        var cases = await casesQuery
            .Select(x => new DashboardCaseProjection(
                x.Status,
                x.Severity,
                x.IsEscalated,
                x.AssignedAnalystId,
                x.CreatedAt,
                x.ResolvedAt))
            .ToListAsync(cancellationToken);

        var activeCases = cases
            .Where(x =>
                x.Status == CaseStatus.Open
                || x.Status == CaseStatus.InReview)
            .ToList();

        var statusCounts = Enum.GetValues<CaseStatus>()
            .Select(status => new StatusCountDto(
                status,
                cases.Count(x => x.Status == status)))
            .ToList();

        var activeSeverityCounts = Enum.GetValues<SeverityLevel>()
            .Select(severity => new SeverityCountDto(
                severity,
                activeCases.Count(x => x.Severity == severity)))
            .ToList();

        var activeEscalatedCases = activeCases
            .Count(x => x.IsEscalated);

        var workloadCounts = activeCases
            .GroupBy(x => x.AssignedAnalystId)
            .Select(group => new
            {
                AnalystId = group.Key,
                ActiveCaseCount = group.Count()
            })
            .ToList();

        var userNames = await _identityService.GetUserNamesAsync(
            workloadCounts.Select(x => x.AnalystId),
            cancellationToken);

        var activeCasesByAnalyst = workloadCounts
            .Select(x => new AnalystWorkloadDto(
                x.AnalystId,
                GetUserName(userNames, x.AnalystId),
                x.ActiveCaseCount))
            .OrderByDescending(x => x.ActiveCaseCount)
            .ThenBy(x => x.AnalystName)
            .ToList();

        var resolutionHours = cases
            .Where(x => x.ResolvedAt.HasValue)
            .Select(x => (x.ResolvedAt!.Value - x.CreatedAt).TotalHours)
            .ToList();

        var averageResolutionHours = resolutionHours.Count == 0
            ? (double?)null
            : Math.Round(
                resolutionHours.Average(),
                2,
                MidpointRounding.AwayFromZero);

        return new DashboardSummaryDto(
            cases.Count,
            statusCounts,
            activeSeverityCounts,
            activeEscalatedCases,
            activeCasesByAnalyst,
            averageResolutionHours);
    }

    private static string GetUserName(
        IReadOnlyDictionary<Guid, string> userNames,
        Guid userId)
    {
        return userNames.TryGetValue(userId, out var userName)
            ? userName
            : UnknownUserName;
    }

    private sealed record DashboardCaseProjection(
        CaseStatus Status,
        SeverityLevel Severity,
        bool IsEscalated,
        Guid AssignedAnalystId,
        DateTime CreatedAt,
        DateTime? ResolvedAt);
}
