using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class DetailContact
    {
        [Parameter]
        public int contactId { get; set; }

        //Inject Services
        [Inject]
        private IContactService contactService { get; set; } = null!;
        [Inject]
        private IWebsiteService websiteService { get; set; } = null!;
        [Inject]
        private IPhoneService phoneService { get; set; } = null!;

        [Inject]
        private NavigationManager navigationManager { get; set; } = null!;

        [Inject]
        private IJSRuntime jsRuntime { get; set; } = null!;

        [Inject]
        private ILogger<DetailContact> logger { get; set; } = null!;

        //Private fields
        private Contact contact = new();
        private List<WebsiteViewModel> websites = new();
        private List<PhoneViewModel> phones = new();
        private bool isInitialized = false;
        private bool showDeleteModal = false;
        private bool isDeleting = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                contact = await contactService.GetByIdAsync(contactId);
                websites = await websiteService.GetByIdAsync(contactId);
                phones = await phoneService.GetByIdAsync(contactId);

                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                userContext = authState.User;
                // ... your existing initialization code
                EmailRequest.To = contact.WorkEmail;
                isInitialized = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading contact with ID {ContactId}", contactId);
                await ShowErrorAsync("Failed to load contact details.");
            }
        }

        private void ShowDeleteConfirmation()
        {
            showDeleteModal = true;
            StateHasChanged();
        }

        private void HideDeleteConfirmation()
        {
            if (!isDeleting)
            {
                showDeleteModal = false;
                StateHasChanged();
            }
        }

        private async Task DeleteContact()
        {
            if (contact == null || isDeleting)
                return;

            try
            {
                isDeleting = true;
                StateHasChanged();

                await contactService.DeleteAsync(contact.Id);

                showDeleteModal = false;

                logger.LogInformation("Successfully deleted contact with ID {ContactId}", contactId);

                // Navigate back to contacts list
                navigationManager.NavigateTo("/contacts");

                // Optional: Show success message using Blazored.Toast if available
                // toastService.ShowSuccess($"Contact {contact.FirstName} {contact.LastName} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting contact with ID {ContactId}", contactId);

                await ShowErrorAsync($"Failed to delete contact: {ex.Message}");

                showDeleteModal = false;
            }
            finally
            {
                isDeleting = false;
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await HandleModalBodyClassAsync();
        }

        private async Task HandleModalBodyClassAsync()
        {
            try
            {
                if (showDeleteModal)
                {
                    await jsRuntime.InvokeVoidAsync("document.body.classList.add", "modal-open");
                }
                else
                {
                    await jsRuntime.InvokeVoidAsync("document.body.classList.remove", "modal-open");
                }
            }
            catch (JSException ex)
            {
                logger.LogWarning(ex, "Failed to manipulate body class for modal");
            }
        }

        private async Task ShowErrorAsync(string message)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("alert", message);
            }
            catch (JSException ex)
            {
                logger.LogWarning(ex, "Failed to show error message via JavaScript");
            }
        }
        private void NavigateToEditPage(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            navigationManager.NavigateTo($"contacts/add/{contact.Id}");
        }
    }
}