using ComplianceSystem.Domain.Enums;

namespace ComplianceSystem.Application.Cases.Dtos;

public sealed record AuditEntryDto(
    Guid Id,
    Guid CaseId,
    AuditActionType ActionType,
    Guid? ActorUserId,
    string ActorName,
    DateTime OccurredAt,
    string? OldValue,
    string? NewValue,
    string? Description);
