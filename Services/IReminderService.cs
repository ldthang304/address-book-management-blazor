using AddressBookManagement.Models;

namespace AddressBookManagement.Services
{
    public interface IReminderService
    {
        Task<List<TodoTask>> GetUpcomingTasksAsync();
    }
}
