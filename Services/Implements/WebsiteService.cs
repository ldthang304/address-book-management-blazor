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
        public async Task<List<WebsiteTypeViewModel>> GetWebsitesWithMasterByContactIdAsync(int contactId)
        {
            throw new NotImplementedException();
            // Step 1: Get all master records with TypeName = "WebsiteType"
            //var websiteTypeMasters = await _masterRepository.Query()
            //    .Where(m => m.TypeName == "WebsiteType")
            //    .ToListAsync();

            //// Step 2: Extract the valid type keys from master
            //var typeKeys = websiteTypeMasters.Select(m => m.TypeKey).ToList();

            //// Step 3: Join website with matching master based on Website.WebsiteType == Master.TypeKey
            //var result = await _websiteRepository.Query()
            //    .Where(w => w.ContactId == contactId && typeKeys.Contains(w.WebsiteType))
            //    .Join(
            //        websiteTypeMasters, // Join with in-memory list
            //        website => website.WebsiteType,
            //        master => master.TypeKey,
            //        (website, master) => new WebsiteTypeViewModel
            //        {
            //            Website = website,
            //            Master = master
            //        })
            //    .ToListAsync();

            //return result;
        }
    }
}
