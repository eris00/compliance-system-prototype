using MediatR;

namespace ComplianceSystem.Application.Cases.Commands.StartReview;

public sealed record StartReviewCommand(Guid CaseId) : IRequest;
