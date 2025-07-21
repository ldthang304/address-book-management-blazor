using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using AddressBookManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AddressBookManagement.Services.Implements
{
    public class PhoneService : IPhoneService
    {
        private readonly IRepository<Phone> _phoneRepository;
        private readonly IRepository<Master> _masterRepository;

        public PhoneService(IRepository<Phone> phoneRepository, IRepository<Master> masterRepository)
        {
            _phoneRepository = phoneRepository;
            _masterRepository = masterRepository;
        }
        public async Task<List<PhoneViewModel>> GetByIdAsync(int id)
        {
            return await _phoneRepository.Query()
                .Join(_masterRepository.Query(),
                    phone => phone.PhoneType,
                    master => master.TypeKey,
                    (phone, master) => new { Phone = phone, Master = master })
                .Where(x => x.Phone.ContactId == id && x.Master.TypeName == "PhoneType")
                .Select(x => new PhoneViewModel
                {
                    Id = x.Phone.Id,
                    Number = x.Phone.Number,
                    PhoneType = x.Master.TypeValue,
                }).ToListAsync();
        }
    }
}
