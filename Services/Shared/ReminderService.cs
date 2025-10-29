using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressBookManagement.Services.Shared
{
    public class ReminderService : IReminderService
    {
        private readonly IRepository<TodoTask> _todoTaskRepository;

        public ReminderService(IRepository<TodoTask> todoTaskRepository)
        {
            _todoTaskRepository = todoTaskRepository;
        }

        public async Task<List<TodoTask>> GetUpcomingTasksAsync()
        {
            var now = DateTime.Now;
            var upcoming = now.AddMinutes(10);

            return await _todoTaskRepository.Query()
                .Where(t => !t.IsCompleted && t.DueDate > now && t.DueDate <= upcoming)
                .ToListAsync();
        }
    }
}
