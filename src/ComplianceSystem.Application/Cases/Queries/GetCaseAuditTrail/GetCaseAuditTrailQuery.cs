using ComplianceSystem.Application.Cases.Dtos;
using MediatR;

namespace ComplianceSystem.Application.Cases.Queries.GetCaseAuditTrail;

public sealed record GetCaseAuditTrailQuery(Guid CaseId)
    : IRequest<IReadOnlyList<AuditEntryDto>>;
