using ComplianceSystem.Domain.Enums;

namespace ComplianceSystem.Application.Cases.Dtos;

public sealed record CaseListItemDto(
    Guid Id,
    string Title,
    CaseStatus Status,
    SeverityLevel Severity,
    Guid CategoryId,
    string CategoryCode,
    string CategoryName,
    Guid AssignedAnalystId,
    string AssignedAnalystName,
    DateTime CreatedAt,
    DateTime DueAt,
    bool IsEscalated);
