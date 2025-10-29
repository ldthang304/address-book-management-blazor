using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using AddressBookManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace AddressBookManagement.Services.Implements
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _contactRepository;
        public ContactService(IRepository<Contact> contactRepository)
        {
            _contactRepository = contactRepository;
        }
        public async Task<Contact> AddAsync(Contact contact)
        {
            return await _contactRepository.AddAsync(contact);
        }

        public async Task<int> CountAsync()
        {
            return await _contactRepository.Query().CountAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _contactRepository.DeleteAsync(id);
        }

        public async Task<List<Contact>> FindAsync(int id)
        {
            return await _contactRepository.FindAsync(c => c.AppUserId == id);
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _contactRepository.GetAllAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _contactRepository.Query().Include(c => c.Phones).Include(c => c.Websites).Include(c => c.Tasks).Include(c => c.Organization).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Contact>> GetFavoritesAsync()
        {
            return await _contactRepository.Query().Where(c => c.IsFavourite == true).ToListAsync();
        }

        public async Task<PageResult<Contact>> GetPagedAsync(int pageIndex, int pageSize, string? sortBy = null, string? sortDirection = "ASC", List<Expression<Func<Contact, bool>>>? filters = null)
        {
            return await _contactRepository.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);
        }

        public async Task RestoreAsync(int id)
        {
            await _contactRepository.RestoreAsync(id);
        }

        public async Task UpdateAsync(Contact contact)
        {
            await _contactRepository.UpdateAsync(contact);
        }
    }
}
