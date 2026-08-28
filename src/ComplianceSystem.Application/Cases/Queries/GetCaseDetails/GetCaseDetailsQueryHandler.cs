using ComplianceSystem.Application.Cases.Dtos;
using ComplianceSystem.Application.Common.Exceptions;
using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Application.Common.Security;
using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Cases.Queries.GetCaseDetails;

public class GetCaseDetailsQueryHandler
    : IRequestHandler<GetCaseDetailsQuery, CaseDetailsDto>
{
    private const string UnknownUserName = "Unknown user";

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetCaseDetailsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<CaseDetailsDto> Handle(
        GetCaseDetailsQuery request,
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
                "Only Analysts, Supervisors and Auditors can view cases.");
        }

        var query = _context.Cases
            .AsNoTracking()
            .Where(x => x.Id == request.CaseId);

        if (!hasGlobalReadScope)
        {
            query = query.Where(x => x.AssignedAnalystId == currentUserId);
        }

        var complianceCase = await query
            .Select(x => new CaseDetailsProjection(
                x.Id,
                x.Title,
                x.Description,
                x.Status,
                x.Severity,
                x.CategoryId,
                x.Category.Code,
                x.Category.Name,
                x.CreatedByUserId,
                x.AssignedAnalystId,
                x.CreatedAt,
                x.DueAt,
                x.IsEscalated,
                x.EscalatedAt,
                x.ResolutionOutcome,
                x.ResolutionExplanation,
                x.ResolvedAt,
                x.ClosedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (complianceCase is null)
        {
            throw new NotFoundException("Case was not found.");
        }

        var userNames = await _identityService.GetUserNamesAsync(
            new[]
            {
                complianceCase.CreatedByUserId,
                complianceCase.AssignedAnalystId
            }.Distinct(),
            cancellationToken);

        return new CaseDetailsDto(
            complianceCase.Id,
            complianceCase.Title,
            complianceCase.Description,
            complianceCase.Status,
            complianceCase.Severity,
            complianceCase.CategoryId,
            complianceCase.CategoryCode,
            complianceCase.CategoryName,
            complianceCase.CreatedByUserId,
            GetUserName(userNames, complianceCase.CreatedByUserId),
            complianceCase.AssignedAnalystId,
            GetUserName(userNames, complianceCase.AssignedAnalystId),
            complianceCase.CreatedAt,
            complianceCase.DueAt,
            complianceCase.IsEscalated,
            complianceCase.EscalatedAt,
            complianceCase.ResolutionOutcome,
            complianceCase.ResolutionExplanation,
            complianceCase.ResolvedAt,
            complianceCase.ClosedAt);
    }

    private static string GetUserName(
        IReadOnlyDictionary<Guid, string> userNames,
        Guid userId)
    {
        return userNames.TryGetValue(userId, out var userName)
            ? userName
            : UnknownUserName;
    }

    private sealed record CaseDetailsProjection(
        Guid Id,
        string Title,
        string Description,
        CaseStatus Status,
        SeverityLevel Severity,
        Guid CategoryId,
        string CategoryCode,
        string CategoryName,
        Guid CreatedByUserId,
        Guid AssignedAnalystId,
        DateTime CreatedAt,
        DateTime DueAt,
        bool IsEscalated,
        DateTime? EscalatedAt,
        string? ResolutionOutcome,
        string? ResolutionExplanation,
        DateTime? ResolvedAt,
        DateTime? ClosedAt);
}
