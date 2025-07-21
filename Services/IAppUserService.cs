using AddressBookManagement.Models;

namespace AddressBookManagement.Services
{
    public interface IAppUserService
    {
        Task<AppUser> GetByEmail(string email);
        Task<List<AppUser>> GetAllAsync();
        Task<AppUser> AddAsync(AppUser user);
        Task DeleteAsync(int id);
    }
}
