using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressBookManagement.Services.Implements
{
    public class NoteService : INoteService
    {
        private readonly IRepository<Note> _noteRepository;

        public NoteService(IRepository<Note> noteRepository)
        {
            _noteRepository = noteRepository;
        }
        public async Task<Note> AddAsync(Note note)
        {
            return await _noteRepository.AddAsync(note);
        }

        public async Task DeleteAsync(int id)
        {
            await _noteRepository.DeleteAsync(id);
        }

        public async Task<List<Note>> GetAllAsync()
        {
            return await _noteRepository.GetAllAsync();
        }

        public async Task<List<Note>?> GetByContactIdAsync(int id)
        {
            return await _noteRepository.Query().Where(n => n.ContactId == id).ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            return await _noteRepository.GetByIdAsync(id);
        }

        public async Task RestoreAsync(int id)
        {
            await _noteRepository.RestoreAsync(id);
        }

        public async Task UpdateAsync(Note note)
        {
            await _noteRepository.UpdateAsync(note);
        }
    }
}
