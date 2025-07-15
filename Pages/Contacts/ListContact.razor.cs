
using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Shared;
using AddressBookManagement.ViewModels;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class ListContact
    {
        //Private fields
        private bool isInitialized = false;
        private List<Master> sortByOptions = new();
        private List<Master> sortDirections = new();
        private PageResult<Contact> pagedResult = new();

        //Filters
        private List<Expression<Func<Contact, bool>>>? filters;

        //Page fields
        private int pageIndex = 0;
        private int pageSize = 4;
        private int totalPages = 0;
        private string? sortBy = "FirstName";
        private string sortDirection = "ASC";
        
        //Inject Services
        [Inject]
        private IContactService ContactService { get; set; } = null!;
        [Inject]
        private IToastService ToastService { get; set; } = null!;
        [Inject]
        private IMasterService MasterService { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private ToastNavigationService ToastNavigationService { get; set; } = null!;
        [Inject]
        private FilterService FilterService { get; set; } = null!;
        [Inject]
        private ILogger<ListContact> Logger { get; set; } = default!;
        //On Initialize method
        protected override async Task OnInitializedAsync()
        {
            //Get Masters
            sortByOptions = await MasterService.GetByTypeNameAsync("SortBy");
            sortDirections = await MasterService.GetByTypeNameAsync("SortDirection");

            //Get Contact page result
            pagedResult = await ContactService.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);

            //Calculate totalPages initially
            totalPages = (int)Math.Ceiling((double)pagedResult.TotalItems / pageSize);

            //Start rendering component
            isInitialized = true;
        }

        //Show Toast message if it has message
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

        //Page Methods
        private async Task GoToPageAsync(int newPageIndex)
        {
            if (newPageIndex >= 0 && newPageIndex != pageIndex)
            {
                pageIndex = newPageIndex;
                pagedResult = await ContactService.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);

                // ⚠️ Update totalPages based on the new result
                totalPages = (int)Math.Ceiling((double)pagedResult.TotalItems / pageSize);

                // 🔁 Clamp the pageIndex if it's now out of range (e.g., page 2 of 1 page)
                if (pageIndex >= totalPages)
                {
                    pageIndex = 0;
                    pagedResult = await ContactService.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);
                }

                StateHasChanged();
            }
        }


        //Next and Previous Page handle
        private async Task NextPage() => await GoToPageAsync(pageIndex + 1);
        private async Task PrevPage() => await GoToPageAsync(pageIndex - 1);

        //Apply Filter
        private async Task ApplyFiler(ContactFilter contactFilter)
        {
            filters = FilterService.Build(contactFilter);
            pageIndex = 0; // 🔁 Reset to page 0 when filter changes
            pagedResult = await ContactService.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);
            totalPages = (int)Math.Ceiling((double)pagedResult.TotalItems / pageSize);
            StateHasChanged();
        }

        private async Task ApplySortBy(ChangeEventArgs e)
        {
            Master? masterSortBy = sortByOptions.FirstOrDefault(m => m.TypeKey == int.Parse(e.Value.ToString()));
            sortBy = masterSortBy?.TypeValue;
            pagedResult = await ContactService.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);
            StateHasChanged();
        }
        private async Task ApplySortDirection(ChangeEventArgs e)
        {
            Master? masterSortDirection = sortDirections.FirstOrDefault(m => m.TypeKey == int.Parse(e.Value.ToString()));
            sortDirection = masterSortDirection?.TypeValue;
            pagedResult = await ContactService.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);
            StateHasChanged();
        }
    }
}