using ComplianceSystem.Domain.Enums;

namespace ComplianceSystem.Application.Cases.Dtos;

public sealed record CaseDetailsDto(
    Guid Id,
    string Title,
    string Description,
    CaseStatus Status,
    SeverityLevel Severity,
    Guid CategoryId,
    string CategoryCode,
    string CategoryName,
    Guid CreatedByUserId,
    string CreatedByUserName,
    Guid AssignedAnalystId,
    string AssignedAnalystName,
    DateTime CreatedAt,
    DateTime DueAt,
    bool IsEscalated,
    DateTime? EscalatedAt,
    string? ResolutionOutcome,
    string? ResolutionExplanation,
    DateTime? ResolvedAt,
    DateTime? ClosedAt);
