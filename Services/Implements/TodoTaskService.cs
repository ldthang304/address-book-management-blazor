using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressBookManagement.Services.Implements
{
    public class TodoTaskService : ITodoTaskService
    {
        private readonly IRepository<TodoTask> _todoTaskRepository;
        public TodoTaskService(IRepository<TodoTask> todoTaskRepository)
        {
            _todoTaskRepository = todoTaskRepository;
        }
        public async Task<TodoTask> AddAsync(TodoTask todoTask)
        {
            return await _todoTaskRepository.AddAsync(todoTask);
        }

        public async Task DeleteAsync(int id)
        {
            await _todoTaskRepository.DeleteAsync(id);
        }

        public async Task<List<TodoTask>> GetAllAsync()
        {
            return await _todoTaskRepository.GetAllAsync();
        }

        public async Task<List<TodoTask>?> GetByContactIdAsync(int id)
        {
            return await _todoTaskRepository.Query().Where(t =>  t.ContactId == id).ToListAsync();
        }

        public async Task<TodoTask?> GetByIdAsync(int id)
        {
            return await _todoTaskRepository.GetByIdAsync(id);
        }

        public async Task RestoreAsync(int id)
        {
            await _todoTaskRepository.RestoreAsync(id);
        }

        public async Task UpdateAsync(TodoTask todoTask)
        {
            await _todoTaskRepository.UpdateAsync(todoTask);
        }
    }
}
