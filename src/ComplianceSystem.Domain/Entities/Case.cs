using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;

namespace ComplianceSystem.Domain.Entities;

public class Case
{
    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public CaseStatus Status { get; private set; }
    public SeverityLevel Severity { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime DueAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    // Escalation
    public bool IsEscalated { get; private set; }
    public DateTime? EscalatedAt { get; private set; }

    // Category
    public Guid CategoryId { get; private set; }
    public CaseCategory Category { get; private set; } = null!;

    // Resolution
    public string? ResolutionOutcome { get; private set; }
    public string? ResolutionExplanation { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    // Identity foreign keys
    public Guid CreatedByUserId { get; private set; }
    public Guid AssignedAnalystId { get; private set; }

    public void Resolve(string outcome, string explanation)
    {
        if (Status != CaseStatus.InReview)
        {
            throw new DomainException(
                "Only a case in review can be resolved.");
        }

        if (string.IsNullOrWhiteSpace(outcome))
        {
            throw new DomainException(
                "Resolution outcome is required.");
        }

        if (string.IsNullOrWhiteSpace(explanation))
        {
            throw new DomainException(
                "Resolution explanation is required.");
        }

        ResolutionOutcome = outcome.Trim();
        ResolutionExplanation = explanation.Trim();
        ResolvedAt = DateTime.UtcNow;
        Status = CaseStatus.Resolved;
    }

    public void Close()
    {
        if (Status == CaseStatus.Closed)
        {
            throw new DomainException("Case is already closed.");
        }

        if (Status != CaseStatus.Resolved)
        {
            throw new DomainException(
                "Only a resolved case can be closed.");
        }

        Status = CaseStatus.Closed;
        ClosedAt = DateTime.UtcNow;
    }
}