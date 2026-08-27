using ComplianceSystem.Application.Common.Interfaces;
using ComplianceSystem.Application.Common.Security;
using ComplianceSystem.Domain.Entities;
using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceSystem.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandHandler
    : IRequestHandler<CreateCaseCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public CreateCaseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<Guid> Handle(
        CreateCaseCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new DomainException("Authenticated user is required.");

        var isSupervisor = _currentUserService.IsInRole(AppRoles.Supervisor);
        var isAnalyst = _currentUserService.IsInRole(AppRoles.Analyst);

        if (!isSupervisor && !isAnalyst)
        {
            throw new DomainException(
                "Only Analysts and Supervisors can create cases.");
        }

        var hasActiveCategory = await _context.CaseCategories
            .AnyAsync(
                category =>
                    category.Id == request.CategoryId
                    && category.IsActive,
                cancellationToken);

        if (!hasActiveCategory)
        {
            throw new DomainException("Active case category was not found.");
        }

        var assignedAnalystId = isSupervisor
            ? await GetSupervisorAssignedAnalystIdAsync(
                request.AssignedAnalystId)
            : GetAnalystAssignedAnalystId(
                currentUserId,
                request.AssignedAnalystId);

        var complianceCase = Case.Create(
            request.Title,
            request.Description,
            request.Severity,
            request.CategoryId,
            currentUserId,
            assignedAnalystId);

        var auditEntry = AuditEntry.Create(
            complianceCase.Id,
            AuditActionType.CaseCreated,
            currentUserId,
            oldValue: null,
            newValue:
                $"Status={CaseStatus.Open}; AssignedAnalystId={assignedAnalystId}",
            description:
                $"Case created and assigned to analyst {assignedAnalystId}.");

        await _context.Cases.AddAsync(
            complianceCase,
            cancellationToken);

        await _context.AuditEntries.AddAsync(
            auditEntry,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return complianceCase.Id;
    }

    private static Guid GetAnalystAssignedAnalystId(
        Guid currentUserId,
        Guid? requestedAssignedAnalystId)
    {
        if (requestedAssignedAnalystId is { } assignedAnalystId
            && assignedAnalystId != currentUserId)
        {
            throw new DomainException(
                "Analysts cannot assign cases to other users.");
        }

        return currentUserId;
    }

    private async Task<Guid> GetSupervisorAssignedAnalystIdAsync(
        Guid? requestedAssignedAnalystId)
    {
        if (requestedAssignedAnalystId is not { } assignedAnalystId
            || assignedAnalystId == Guid.Empty)
        {
            throw new DomainException(
                "Assigned analyst ID is required for Supervisors.");
        }

        var userExists = await _identityService.UserExistsAsync(
            assignedAnalystId);

        if (!userExists)
        {
            throw new DomainException("Assigned analyst was not found.");
        }

        var isAnalyst = await _identityService.IsInRoleAsync(
            assignedAnalystId,
            AppRoles.Analyst);

        if (!isAnalyst)
        {
            throw new DomainException(
                "Assigned user must have the Analyst role.");
        }

        return assignedAnalystId;
    }
}
