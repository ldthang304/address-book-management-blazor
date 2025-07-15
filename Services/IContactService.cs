using AddressBookManagement.Models;
using AddressBookManagement.ViewModels;
using System.Linq.Expressions;

namespace AddressBookManagement.Services
{
    public interface IContactService
    {
        Task<List<Contact>> GetAllAsync();
        Task<PageResult<Contact>> GetPagedAsync(int pageIndex, int pageSize, string? sortBy = null, string? sortDirection = "ASC", List<Expression<Func<Contact, bool>>>? filters = null);
        Task<int> CountAsync();
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact> AddAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<List<Contact>> GetFavoritesAsync();
    }
}
