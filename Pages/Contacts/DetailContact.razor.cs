using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.ViewModels;
using Microsoft.AspNetCore.Components;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class DetailContact
    {
        [Parameter]
        public int contactId { get; set; }

        //Inject Services
        [Inject]
        private IContactService contactService { get; set; } = null!;

        //Private fields
        private Contact contact = new();
        private bool isInitialized = false;
        private List<WebsiteTypeViewModel> websiteTypeViewModels = new();

        protected override async Task OnInitializedAsync()
        {
            contact = await contactService.GetByIdAsync(contactId);
                      
            isInitialized = true;
        }

    }
}