using AddressBookManagement.ViewModels;

namespace AddressBookManagement.Services
{
    public interface IPhoneService
    {
        //Get Website with Master
        Task<List<PhoneViewModel>> GetByIdAsync(int id);
    }
}
