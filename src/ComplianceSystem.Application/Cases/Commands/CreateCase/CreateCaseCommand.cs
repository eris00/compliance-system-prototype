using ComplianceSystem.Domain.Enums;
using MediatR;

namespace ComplianceSystem.Application.Cases.Commands.CreateCase;

public record CreateCaseCommand(
    string Title,
    string Description,
    SeverityLevel Severity,
    Guid CategoryId,
    Guid? AssignedAnalystId) : IRequest<Guid>;
