using ComplianceSystem.Application.Cases.Dtos;
using MediatR;

namespace ComplianceSystem.Application.Cases.Queries.GetCaseDetails;

public sealed record GetCaseDetailsQuery(Guid CaseId)
    : IRequest<CaseDetailsDto>;
