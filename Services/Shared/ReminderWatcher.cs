using AddressBookManagement.Models;
using AddressBookManagement.Services;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

public class ReminderWatcher : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReminderWatcher> _logger;
    private Timer? _timer;
    private List<int> _remindedTaskIds = new();

    public event Action<List<TodoTask>>? OnReminderTriggered;

    public ReminderWatcher(
        IServiceScopeFactory scopeFactory,
        ILogger<ReminderWatcher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        _timer = new Timer(async _ => await CheckRemindersAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    private async Task CheckRemindersAsync()
    {
        try
        {
            _logger.LogInformation("🔄 Reminder check started at {Time}", DateTime.Now);

            using var scope = _scopeFactory.CreateScope();
            var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();

            var tasks = await reminderService.GetUpcomingTasksAsync();

            var newTasks = tasks
                .Where(t => !_remindedTaskIds.Contains(t.Id))
                .ToList();

            if (newTasks.Any())
            {
                _logger.LogInformation("🔔 {Count} new reminder(s) found at {Time}", newTasks.Count, DateTime.Now);
                _remindedTaskIds.AddRange(newTasks.Select(t => t.Id));
                OnReminderTriggered?.Invoke(newTasks);
            }
            else
            {
                _logger.LogInformation("✅ No new reminders at {Time}", DateTime.Now);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during reminder check at {Time}", DateTime.Now);
        }
    }


    public void Dispose()
    {
        _timer?.Dispose();
    }
}
