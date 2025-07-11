using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Implements;
using AddressBookManagement.Services.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class AddContact
    {
        //Initialize flag 
        private bool _isInitialized = false;
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
        private Contact? _contact;
        
        private List<Organization>? _organizations;

        private List<Master>? _masters;

        //MasterMap after grouping all masters
        private Dictionary<string, List<Master>>? MasterMap;

        protected override async Task OnInitializedAsync()
        {
            //Initialize
            _masters = new List<Master>();
            _organizations = new List<Organization>();
            _contact = new Contact();

            _masters = await MasterService.GetAllAsync();
            GroupMasters();
            _organizations = await OrganizationService.GetAllAsync();
            
            if (ContactId.HasValue)
            {
                _contact = await ContactService.GetByIdAsync(ContactId.Value);
            }
            //Finish initializing
            _isInitialized = true;
        }

        private void GroupMasters()
        {
            MasterMap = _masters
                .GroupBy(m => m.TypeName)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        //Save Contact into database
        private async Task SaveAsync()
        {
            if (!ContactId.HasValue && _contact != null)
            {
                await ContactService.AddAsync(_contact);
                ToastNavigationService.SetMessage("Contact added successfully", ToastLevel.Success);
            }
            else
            {
                await ContactService.UpdateAsync(_contact!);
                ToastNavigationService.SetMessage("Contact updated successfully", ToastLevel.Success);

            }
            //Navigate to contact list after added
            NavigationManager.NavigateTo("contacts");
        }

        private bool IsPhoneAdded(Master phoneType)
        {
            return _contact?.Phones?.FirstOrDefault(p => p.PhoneType == phoneType.TypeKey) != null;
        }

        private void AddPhoneWithType(Master type)
        {
            if (_contact?.Phones == null)
            {
                _contact?.Phones = new List<Phone>();
            }
            _contact?.Phones?.Add(new Phone()
            {
                Number = "",
                PhoneType = type.TypeKey,
                ContactId = _contact.Id,
                Contact = _contact

            });
        }
        private void RemovePhone(Phone phone)
        {
            _contact?.Phones?.Remove(phone);
        }

        private bool IsWebsiteAdded(Master websiteType)
        {
            return _contact?.Websites?.FirstOrDefault(w => w.WebsiteType == websiteType.TypeKey) != null;
        }

        private void AddWebsiteWithType(Master type)
        {
            if (_contact?.Websites == null)
            {
                _contact?.Websites = new List<Website>();
            }

            _contact?.Websites?.Add(new Website()
            {
                Url = "",
                ContactId = _contact.Id,
                WebsiteType = type.TypeKey,
                Contact = _contact
            });
        }
        private void RemoveWebsite(Website website)
        {
            _contact?.Websites?.Remove(website);
        }
        private void BackToList(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("contacts");
        }
    }
}