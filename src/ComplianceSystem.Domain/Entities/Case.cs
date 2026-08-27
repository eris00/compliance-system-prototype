using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;

namespace ComplianceSystem.Domain.Entities;

public class Case
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;

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

    private Case()
    {
    }

    public static Case Create(
        string title,
        string description,
        SeverityLevel severity,
        Guid categoryId,
        Guid createdByUserId,
        Guid assignedAnalystId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Case title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Case description is required.");
        }

        var trimmedTitle = title.Trim();
        var trimmedDescription = description.Trim();

        if (trimmedTitle.Length > MaxTitleLength)
        {
            throw new DomainException(
                $"Case title cannot exceed {MaxTitleLength} characters.");
        }

        if (trimmedDescription.Length > MaxDescriptionLength)
        {
            throw new DomainException(
                $"Case description cannot exceed {MaxDescriptionLength} characters.");
        }

        if (!Enum.IsDefined(severity))
        {
            throw new DomainException("Case severity is invalid.");
        }

        if (categoryId == Guid.Empty)
        {
            throw new DomainException("Case category ID is required.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Created by user ID is required.");
        }

        if (assignedAnalystId == Guid.Empty)
        {
            throw new DomainException("Assigned analyst ID is required.");
        }

        var createdAt = DateTime.UtcNow;

        return new Case
        {
            Id = Guid.NewGuid(),
            Title = trimmedTitle,
            Description = trimmedDescription,
            Status = CaseStatus.Open,
            Severity = severity,
            CreatedAt = createdAt,
            DueAt = CalculateDueAt(createdAt, severity),
            ClosedAt = null,
            IsEscalated = false,
            EscalatedAt = null,
            CategoryId = categoryId,
            ResolutionOutcome = null,
            ResolutionExplanation = null,
            ResolvedAt = null,
            CreatedByUserId = createdByUserId,
            AssignedAnalystId = assignedAnalystId
        };
    }

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

    private static DateTime CalculateDueAt(
        DateTime createdAt,
        SeverityLevel severity)
    {
        return severity switch
        {
            SeverityLevel.Critical => createdAt.AddHours(24),
            SeverityLevel.High => createdAt.AddHours(24),
            SeverityLevel.Medium => createdAt.AddDays(3),
            SeverityLevel.Low => createdAt.AddDays(7),
            _ => throw new DomainException(
                "Due date rule is not configured for the selected severity.")
        };
    }
}
