using MediatR;

namespace ComplianceSystem.Application.Cases.Commands.CloseCase;

public sealed record CloseCaseCommand(Guid CaseId) : IRequest;
