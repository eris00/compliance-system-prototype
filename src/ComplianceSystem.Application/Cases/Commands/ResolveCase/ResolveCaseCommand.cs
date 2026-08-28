using MediatR;

namespace ComplianceSystem.Application.Cases.Commands.ResolveCase;

public sealed record ResolveCaseCommand(
    Guid CaseId,
    string Outcome,
    string Explanation) : IRequest;
