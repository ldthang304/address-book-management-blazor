using Microsoft.AspNetCore.Components;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class DetailContact
    {
        [Parameter]
        public int ContactId { get; set; }
    }
}