namespace ComplianceSystem.Application.Common.Security;

public static class AppRoles
{
    public const string Analyst = "Analyst";

    public const string Supervisor = "Supervisor";

    public const string Auditor = "Auditor";

    public static readonly string[] All =
    [
        Analyst,
        Supervisor,
        Auditor
    ];
}
