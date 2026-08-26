namespace ComplianceSystem.Application.Authentication.Models;

public record AuthenticatedUser(
    string Id,
    string Email);