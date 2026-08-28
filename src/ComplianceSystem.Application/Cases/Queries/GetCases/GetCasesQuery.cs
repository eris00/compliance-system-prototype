using ComplianceSystem.Application.Cases.Dtos;
using ComplianceSystem.Domain.Enums;
using MediatR;

namespace ComplianceSystem.Application.Cases.Queries.GetCases;

public sealed record GetCasesQuery(
    CaseStatus? Status,
    SeverityLevel? Severity,
    Guid? CategoryId,
    bool? IsEscalated,
    Guid? AssignedAnalystId)
    : IRequest<IReadOnlyList<CaseListItemDto>>;
