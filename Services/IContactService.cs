using AddressBookManagement.Models;

namespace AddressBookManagement.Services
{
    public interface IContactService
    {
        Task<List<Contact>> GetAllAsync();
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact> AddAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<List<Contact>> GetFavoritesAsync();
    }
}
