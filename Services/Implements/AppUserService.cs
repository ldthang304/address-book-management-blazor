using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace AddressBookManagement.Services.Implements
{
    public class AppUserService : IAppUserService
    {
        private readonly IRepository<AppUser> _repository;
        public AppUserService(IRepository<AppUser> repository)
        {
            _repository = repository;
        }

        public async Task<AppUser> AddAsync(AppUser user)
        {
            return await _repository.AddAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<AppUser>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<AppUser> GetByEmail(string email)
        {
            return await _repository.Query().FirstOrDefaultAsync(u => u.Email.Equals(email));
        }
    }
}
