using MediatR;

namespace ComplianceSystem.Application.Cases.Commands.EscalateOverdueCases;

public sealed record EscalateOverdueCasesCommand : IRequest<int>;
