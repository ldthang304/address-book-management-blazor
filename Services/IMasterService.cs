using AddressBookManagement.Models;

namespace AddressBookManagement.Services
{
    public interface IMasterService
    {
        Task<List<Master>> GetAllAsync();
        Task<Master?> GetByIdAsync(int id);
        Task<Master> AddAsync(Master master);
        Task UpdateAsync(Master master);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}
