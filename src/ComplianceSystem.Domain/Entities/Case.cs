
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
    public DateTime? ClosedAt { get; private set; }
    public Guid CategoryId { get; private set; }

    public void Close()
    {
        if (Status == CaseStatus.Closed)
        {
            throw new DomainException("Case is already closed");
        }

        Status = CaseStatus.Closed;
        ClosedAt = DateTime.UtcNow;
    }
}