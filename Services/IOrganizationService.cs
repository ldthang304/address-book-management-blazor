using AddressBookManagement.Models;

namespace AddressBookManagement.Services
{
    public interface IOrganizationService
    {
        Task<List<Organization>> GetAllAsync();
        Task<Organization?> GetByIdAsync(int id);
        Task<Organization> AddAsync(Organization organization);
        Task UpdateAsync(Organization organization);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
    }
}
