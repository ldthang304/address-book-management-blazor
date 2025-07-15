using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class AddContact
    {
        //Initialize flag 
        private bool isInitialized = false;
        //Inject Services
        [Inject]
        private IContactService ContactService { get; set; } = null!;
        [Inject]
        private IOrganizationService OrganizationService { get; set; } = null!;
        [Inject]
        private IMasterService MasterService { get; set; } = null!;
        [Inject]
        private ToastNavigationService ToastNavigationService { get; set; } = null!;
        [Inject] 
        private NavigationManager NavigationManager { get; set; } = null!;
        
        [Parameter]
        //Contact Id from route
        public int? ContactId { get; set; }

        //Contact to make operations on
        private Contact contact = new();
        
        private List<Organization> organizations = new();

        private List<Master> masters = new();

        //MasterMap after grouping all masters
        private Dictionary<string, List<Master>>? MasterMap;

        protected override async Task OnInitializedAsync()
        {
            masters = await MasterService.GetAllAsync();
            GroupMasters();
            organizations = await OrganizationService.GetAllAsync();
            
            if (ContactId.HasValue)
            {
                contact = await ContactService.GetByIdAsync(ContactId.Value);
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

        //Save Contact into database
        private async Task SaveAsync()
        {
            if (!ContactId.HasValue && contact != null)
            {
                await ContactService.AddAsync(contact);
                ToastNavigationService.SetMessage("Contact added successfully", ToastLevel.Success);
            }
            else
            {
                await ContactService.UpdateAsync(contact!);
                ToastNavigationService.SetMessage("Contact updated successfully", ToastLevel.Success);

            }
            //Navigate to contact list after added
            NavigationManager.NavigateTo("contacts");
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
        private void BackToList(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("contacts");
        }
    }
}