using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Shared;
using AddressBookManagement.ViewModels;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class ListContact
    {
        //Private fields
        private bool isInitialized = false;
        private List<Master> sortByOptions = new();
        private List<Master> sortDirections = new();
        private PageResult<Contact> pagedResult = new();
        private ClaimsPrincipal? userContext;

        //Filters
        private List<Expression<Func<Contact, bool>>>? filters = new();

        //Page fields
        private int pageIndex = 0;
        private int pageSize = 4;
        private int totalPages = 0;
        private string? sortBy = "CreatedAt";
        private string sortDirection = "DESC";
        private int userId;

        //Search fields
        private string searchTerm = string.Empty;
        private Timer? searchTimer;
        private readonly int searchDelay = 300; // milliseconds delay for debouncing

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
        private FilterService FilterService { get; set; } = null!;
        [Inject]
        private ILogger<ListContact> Logger { get; set; } = default!;

        //On Initialize method
        protected override async Task OnInitializedAsync()
        {
            //Get user in authen state
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            userContext = authState.User;
            userId = int.Parse(userContext.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value); 

            //Get Masters
            sortByOptions = await MasterService.GetByTypeNameAsync("SortBy");
            sortDirections = await MasterService.GetByTypeNameAsync("SortDirection");

            //Add initial filters: filter by user id
            filters?.Add(c => c.AppUserId == userId);

            //Get Contact page result
            pagedResult = await ContactService.GetPagedAsync(pageIndex, pageSize, sortBy, sortDirection, filters);

            //Calculate totalPages initially
            totalPages = (int)Math.Ceiling((double)pagedResult.TotalItems / pageSize);

            //Start rendering component
            isInitialized = true;
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

        //Real-time Search Methods
        private void OnSearchInput(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;

            // Dispose existing timer
            searchTimer?.Dispose();

            // Create new timer with debounce delay
            searchTimer = new Timer(async _ => await PerformSearch(), null, searchDelay, Timeout.Infinite);
        }

        private async Task PerformSearch()
        {
            await InvokeAsync(async () =>
            {
                try
                {
                    // Build search filters
                    var searchFilters = FilterService.BuildSearchFilters(searchTerm);

                    // Combine with existing filters
                    var combinedFilters = new List<Expression<Func<Contact, bool>>>();
                    if (filters != null && filters.Any())
                        combinedFilters.AddRange(filters);

                    // Add search filter if exists
                    if (searchFilters != null && searchFilters.Any())
                    {
                        // If you need OR logic for search terms, you might need to modify this
                        // For now, this adds each search filter separately (AND logic)
                        combinedFilters.AddRange(searchFilters);
                    }

                    // Reset to first page when searching
                    pageIndex = 0;

                    // Get filtered results
                    pagedResult = await ContactService.GetPagedAsync(
                        pageIndex,
                        pageSize,
                        sortBy,
                        sortDirection,
                        combinedFilters.Any() ? combinedFilters : null);

                    // Update pagination
                    totalPages = (int)Math.Ceiling((double)pagedResult.TotalItems / pageSize);

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error performing search for term: {SearchTerm}", searchTerm);
                    ToastService.ShowError("An error occurred while searching contacts.");
                }
            });
        }

        private void ClearSearch()
        {
            searchTerm = string.Empty;
            searchTimer?.Dispose();

            // Trigger search with empty term to reset results
            _ = Task.Run(async () => await PerformSearch());
        }

        // Dispose timer when component is disposed
        public void Dispose()
        {
            searchTimer?.Dispose();
        }
    }
}