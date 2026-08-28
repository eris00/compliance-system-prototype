using ComplianceSystem.Application.Cases.Commands.EscalateOverdueCases;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ComplianceSystem.Infrastructure.BackgroundServices;

public class AutomaticEscalationBackgroundService : BackgroundService
{
    private const int DefaultCheckIntervalSeconds = 30;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AutomaticEscalationBackgroundService> _logger;
    private readonly TimeSpan _checkInterval;

    public AutomaticEscalationBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AutomaticEscalationBackgroundService> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        var intervalSeconds = configuration.GetValue<int?>(
            "Escalation:CheckIntervalSeconds");

        _checkInterval = TimeSpan.FromSeconds(
            intervalSeconds is > 0
                ? intervalSeconds.Value
                : DefaultCheckIntervalSeconds);
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await ProcessEscalationsAsync(stoppingToken);

        using var timer = new PeriodicTimer(_checkInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessEscalationsAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task ProcessEscalationsAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var sender = scope.ServiceProvider
                .GetRequiredService<ISender>();

            var escalatedCount = await sender.Send(
                new EscalateOverdueCasesCommand(),
                stoppingToken);

            if (escalatedCount > 0)
            {
                _logger.LogInformation(
                    "Automatically escalated {EscalatedCount} overdue cases.",
                    escalatedCount);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to automatically escalate overdue cases.");
        }
    }
}
