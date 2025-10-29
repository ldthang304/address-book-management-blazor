using AddressBookManagement.ViewModels;

namespace AddressBookManagement.Services
{
    public interface IWebsiteService
    {
        //Get Website with Master
        Task<List<WebsiteViewModel>> GetByIdAsync(int id);
    }
}
