using AddressBookManagement.Models;

namespace AddressBookManagement.Services
{
    public interface ITodoTaskService
    {
        Task<List<TodoTask>> GetAllAsync();
        Task<TodoTask?> GetByIdAsync(int id);
        Task<List<TodoTask>?> GetByContactIdAsync(int id);
        Task<TodoTask> AddAsync(TodoTask master);
        Task UpdateAsync(TodoTask master);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);

    }
}
