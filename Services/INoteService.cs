using AddressBookManagement.Models;

namespace AddressBookManagement.Services
{
    public interface INoteService
    {
        Task<List<Note>> GetAllAsync();
        Task<Note?> GetByIdAsync(int id);
        Task<List<Note>?> GetByContactIdAsync(int id);
        Task<Note> AddAsync(Note note);
        Task UpdateAsync(Note note);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}
