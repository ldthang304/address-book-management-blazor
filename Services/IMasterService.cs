using AddressBookManagement.Models;
using AddressBookManagement.ViewModels;

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
        
        //Get Master by Type Name
        Task<List<Master>> GetByTypeNameAsync(string typeName);
        
    }
}
