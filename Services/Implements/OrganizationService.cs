using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;

namespace AddressBookManagement.Services.Implements
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IRepository<Organization> _organizationRepository;
        public OrganizationService(IRepository<Organization> organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }
        public async Task<Organization> AddAsync(Organization organization)
        {
            return await _organizationRepository.AddAsync(organization);
        }

        public async Task DeleteAsync(int id)
        {
            await _organizationRepository.DeleteAsync(id);
        }
        public async Task<List<Organization>> GetAllAsync()
        {
            return await _organizationRepository.GetAllAsync();
        }

        public async Task<Organization?> GetByIdAsync(int id)
        {
            return await _organizationRepository.GetByIdAsync(id);
        }

        public async Task RestoreAsync(int id)
        {
            await _organizationRepository.RestoreAsync(id);
        }

        public async Task UpdateAsync(Organization organization)
        {
            await _organizationRepository.UpdateAsync(organization);
        }
    }
}
