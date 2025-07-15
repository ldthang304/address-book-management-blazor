using AddressBookManagement.ViewModels;

namespace AddressBookManagement.Services
{
    public interface IWebsiteService
    {
        //Get Website with Master
        Task<List<WebsiteTypeViewModel>> GetWebsitesWithMasterByContactIdAsync(int contactId);
    }
}
