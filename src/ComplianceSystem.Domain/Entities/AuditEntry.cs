using ComplianceSystem.Domain.Enums;
using ComplianceSystem.Domain.Exceptions;

namespace ComplianceSystem.Domain.Entities;

public class AuditEntry
{
    public Guid Id { get; private set; }

    public Guid CaseId { get; private set; }

    public AuditActionType ActionType { get; private set; }

    public Guid? ActorUserId { get; private set; }

    public DateTime OccurredAt { get; private set; }

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }

    public string? Description { get; private set; }

    private AuditEntry()
    {
    }

    public static AuditEntry Create(
        Guid caseId,
        AuditActionType actionType,
        Guid? actorUserId,
        string? oldValue = null,
        string? newValue = null,
        string? description = null)
    {
        if (caseId == Guid.Empty)
        {
            throw new DomainException("Case ID is required.");
        }

        return new AuditEntry
        {
            Id = Guid.NewGuid(),
            CaseId = caseId,
            ActionType = actionType,
            ActorUserId = actorUserId,
            OccurredAt = DateTime.UtcNow,
            OldValue = oldValue,
            NewValue = newValue,
            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim()
        };
    }
}