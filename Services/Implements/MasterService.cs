using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;

namespace AddressBookManagement.Services.Implements
{
    public class MasterService : IMasterService
    {
        private readonly IRepository<Master> _masterRepository;
        public MasterService(IRepository<Master> masterRepository)
        {
            _masterRepository = masterRepository;
        }
        public async Task<Master> AddAsync(Master master)
        {
            return await _masterRepository.AddAsync(master);
        }

        public async Task DeleteAsync(int id)
        {
            await _masterRepository.DeleteAsync(id);
        }
        public async Task<List<Master>> GetAllAsync()
        {
            return await _masterRepository.GetAllAsync();
        }

        public async Task<Master?> GetByIdAsync(int id)
        {
            return await _masterRepository.GetByIdAsync(id);
        }

        public async Task<List<Master>> GetByTypeNameAsync(string typeName)
        {
            return await _masterRepository.FindAsync(m => m.TypeName!.Equals(typeName));
        }

        public async Task RestoreAsync(int id)
        {
            await _masterRepository.RestoreAsync(id);
        }

        public async Task UpdateAsync(Master master)
        {
            await _masterRepository.UpdateAsync(master);
        }
    }
}
