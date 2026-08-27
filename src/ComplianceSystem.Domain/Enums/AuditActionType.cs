namespace ComplianceSystem.Domain.Enums;

public enum AuditActionType
{
    CaseCreated = 1,
    CaseReassigned = 2,
    ReviewStarted = 3,
    CaseResolved = 4,
    CaseClosed = 5,
    CaseEscalated = 6
}