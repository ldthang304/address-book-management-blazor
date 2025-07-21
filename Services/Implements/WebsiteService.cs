using AddressBookManagement.Datas.Repositories;
using AddressBookManagement.Models;
using AddressBookManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AddressBookManagement.Services.Implements
{
    public class WebsiteService : IWebsiteService
    {
        private readonly IRepository<Website> _websiteRepository;
        private readonly IRepository<Master> _masterRepository;
        public WebsiteService(IRepository<Website> websiteRepository, IRepository<Master> masterRepository)
        {
            _websiteRepository = websiteRepository;
            _masterRepository = masterRepository;
        }

        public async Task<List<WebsiteViewModel>> GetByIdAsync(int id)
        {
            return await _websiteRepository.Query()
                .Join(_masterRepository.Query(),
                    website => website.WebsiteType,
                    master => master.TypeKey,
                    (website, master) => new { Website = website, Master = master })
                .Where(x => x.Website.ContactId == id && x.Master.TypeName == "WebsiteType")
                .Select(x => new WebsiteViewModel
                {
                    Id = x.Website.Id,
                    Url = x.Website.Url,
                    WebsiteType = x.Master.TypeValue,
                })
                .ToListAsync();
        }

    }
}
