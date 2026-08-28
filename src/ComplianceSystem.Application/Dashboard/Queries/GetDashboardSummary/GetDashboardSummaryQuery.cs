using MediatR;

namespace ComplianceSystem.Application.Dashboard.Queries.GetDashboardSummary;

public sealed record GetDashboardSummaryQuery
    : IRequest<DashboardSummaryDto>;
