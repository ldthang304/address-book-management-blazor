using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteAsync(int id)
        {
            await _contactRepository.DeleteAsync(id);
        }
        public async Task<List<Contact>> GetAllAsync()
        {
            return await _contactRepository.GetAllAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _contactRepository.Query().Include(c => c.Phones).Include(c => c.Websites).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Contact>> GetFavoritesAsync()
        {
            return await _contactRepository.Query().Where(c => c.IsFavourite == true).ToListAsync();
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
