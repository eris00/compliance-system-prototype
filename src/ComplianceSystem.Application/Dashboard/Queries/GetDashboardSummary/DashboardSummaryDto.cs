using ComplianceSystem.Domain.Enums;

namespace ComplianceSystem.Application.Dashboard.Queries.GetDashboardSummary;

public sealed record StatusCountDto(
    CaseStatus Status,
    int Count);

public sealed record SeverityCountDto(
    SeverityLevel Severity,
    int Count);

public sealed record AnalystWorkloadDto(
    Guid AnalystId,
    string AnalystName,
    int ActiveCaseCount);

public sealed record DashboardSummaryDto(
    int TotalCases,
    IReadOnlyList<StatusCountDto> CasesByStatus,
    IReadOnlyList<SeverityCountDto> ActiveCasesBySeverity,
    int ActiveEscalatedCases,
    IReadOnlyList<AnalystWorkloadDto> ActiveCasesByAnalyst,
    double? AverageResolutionHours);
