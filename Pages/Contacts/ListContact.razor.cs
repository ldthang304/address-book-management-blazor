
using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class ListContact
    {
        //Private fields
        private bool _isInitialized = false;
        private string _contentTitle = "LIST CONTACT";
        private List<Contact>? _contacts;
        //Inject Services
        [Inject]
        private IContactService ContactService { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private IToastService ToastService { get; set; } = null!;
        [Inject]
        private ToastNavigationService ToastNavigationService { get; set; } = null!;
        protected override async Task OnInitializedAsync()
        {
            _contacts = await ContactService.GetAllAsync();
            _isInitialized = true;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var (message, level) = ToastNavigationService.ConsumeMessage();
            if (!string.IsNullOrEmpty(message))
            {
                await Task.Delay(100);
                ToastService.ShowToast(level, message);
            }
        }

        //Handle Card Click
        private void HandleCardClicked(Contact contact)
        {
            NavigationManager.NavigateTo($"/contacts/add/{contact.Id}");
        }
    }
}