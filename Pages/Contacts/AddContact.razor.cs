using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Implements;
using Microsoft.AspNetCore.Components;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class AddContact
    {
        //Initialize flag 
        private bool isInitialized = false;
        [Inject]
        private IContactService contactService { get; set; } = null!;
        [Inject]
        private IOrganizationService organizationService { get; set; } = null!;
        [Inject]
        private IMasterService masterService { get; set; } = null!;
        [Parameter]
        //Contact Id from route
        public int? ContactId { get; set; }

        //Contact to make operations on
        private Contact? contact;
        
        private List<Organization>? organizations;

        private List<Master>? masters;

        //MasterMap after grouping all masters
        private Dictionary<string, List<Master>>? MasterMap;

        protected override async Task OnInitializedAsync()
        {
            //Initialize
            masters = new List<Master>();
            organizations = new List<Organization>();
            contact = new Contact();

            masters = await masterService.GetAllAsync();
            GroupMasters();
            organizations = await organizationService.GetAllAsync();
            
            if (ContactId.HasValue)
            {
                contact = await contactService.GetByIdAsync(ContactId.Value);
            }
            //Finish initializing
            isInitialized = true;
        }

        private void GroupMasters()
        {
            MasterMap = masters
                .GroupBy(m => m.TypeName)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private void Save()
        {
            Console.WriteLine(contact);
        }

        private bool IsPhoneAdded(Master phoneType)
        {
            return contact?.Phones?.FirstOrDefault(p => p.PhoneType == phoneType.TypeKey) != null;
        }

        private void AddPhoneWithType(Master type)
        {
            if (contact?.Phones == null)
            {
                contact?.Phones = new List<Phone>();
            }
            contact?.Phones?.Add(new Phone()
            {
                Number = "",
                PhoneType = type.TypeKey,
                ContactId = contact.Id,
                Contact = contact

            });
        }
        private void RemovePhone(Phone phone)
        {
            contact?.Phones?.Remove(phone);
        }

        private bool IsWebsiteAdded(Master websiteType)
        {
            return contact?.Websites?.FirstOrDefault(w => w.WebsiteType == websiteType.TypeKey) != null;
        }

        private void AddWebsiteWithType(Master type)
        {
            if (contact?.Websites == null)
            {
                contact?.Websites = new List<Website>();
            }

            contact?.Websites?.Add(new Website()
            {
                Url = "",
                ContactId = contact.Id,
                WebsiteType = type.TypeKey,
                Contact = contact
            });
        }
        private void RemoveWebsite(Website website)
        {
            contact?.Websites?.Remove(website);
        }
    }
}