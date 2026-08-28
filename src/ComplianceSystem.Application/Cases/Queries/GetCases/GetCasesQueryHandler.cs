using ComplianceSystem.Application.Cases.Dtos;
using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Application.Common.Security;
using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Cases.Queries.GetCases;

public class GetCasesQueryHandler
    : IRequestHandler<GetCasesQuery, IReadOnlyList<CaseListItemDto>>
{
    private const string UnknownUserName = "Unknown user";

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetCasesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<IReadOnlyList<CaseListItemDto>> Handle(
        GetCasesQuery request,
        CancellationToken cancellationToken)
    {
        ValidateEnumFilters(request);

        var currentUserId = _currentUserService.UserId
            ?? throw new DomainException("Authenticated user is required.");

        var hasGlobalReadScope =
            _currentUserService.IsInRole(AppRoles.Supervisor)
            || _currentUserService.IsInRole(AppRoles.Auditor);

        var isAnalyst = _currentUserService.IsInRole(AppRoles.Analyst);

        if (!hasGlobalReadScope && !isAnalyst)
        {
            throw new DomainException(
                "Only Analysts, Supervisors and Auditors can view cases.");
        }

        var query = _context.Cases
            .AsNoTracking();

        if (!hasGlobalReadScope)
        {
            query = query.Where(x => x.AssignedAnalystId == currentUserId);
        }
        else if (request.AssignedAnalystId is { } assignedAnalystId)
        {
            query = query.Where(x => x.AssignedAnalystId == assignedAnalystId);
        }

        if (request.Status is { } status)
        {
            query = query.Where(x => x.Status == status);
        }

        if (request.Severity is { } severity)
        {
            query = query.Where(x => x.Severity == severity);
        }

        if (request.CategoryId is { } categoryId)
        {
            query = query.Where(x => x.CategoryId == categoryId);
        }

        if (request.IsEscalated is { } isEscalated)
        {
            query = query.Where(x => x.IsEscalated == isEscalated);
        }

        var cases = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new CaseListItemProjection(
                x.Id,
                x.Title,
                x.Status,
                x.Severity,
                x.CategoryId,
                x.Category.Code,
                x.Category.Name,
                x.AssignedAnalystId,
                x.CreatedAt,
                x.DueAt,
                x.IsEscalated))
            .ToListAsync(cancellationToken);

        if (cases.Count == 0)
        {
            return [];
        }

        var userNames = await _identityService.GetUserNamesAsync(
            cases.Select(x => x.AssignedAnalystId).Distinct(),
            cancellationToken);

        return cases
            .Select(x => new CaseListItemDto(
                x.Id,
                x.Title,
                x.Status,
                x.Severity,
                x.CategoryId,
                x.CategoryCode,
                x.CategoryName,
                x.AssignedAnalystId,
                GetUserName(userNames, x.AssignedAnalystId),
                x.CreatedAt,
                x.DueAt,
                x.IsEscalated))
            .ToList();
    }

    private static void ValidateEnumFilters(GetCasesQuery request)
    {
        if (request.Status is { } status
            && !Enum.IsDefined(status))
        {
            throw new DomainException("Case status filter is invalid.");
        }

        if (request.Severity is { } severity
            && !Enum.IsDefined(severity))
        {
            throw new DomainException("Case severity filter is invalid.");
        }
    }

    private static string GetUserName(
        IReadOnlyDictionary<Guid, string> userNames,
        Guid userId)
    {
        return userNames.TryGetValue(userId, out var userName)
            ? userName
            : UnknownUserName;
    }

    private sealed record CaseListItemProjection(
        Guid Id,
        string Title,
        CaseStatus Status,
        SeverityLevel Severity,
        Guid CategoryId,
        string CategoryCode,
        string CategoryName,
        Guid AssignedAnalystId,
        DateTime CreatedAt,
        DateTime DueAt,
        bool IsEscalated);
}
