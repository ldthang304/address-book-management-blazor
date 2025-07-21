namespace AddressBookManagement.Models
{
    public class ContactFilter
    {
        public int? UserIdFilter { get; set; }
        public string? NameFilter { get; set; }
        public string? JobTitleFilter { get; set; }
        public int? OrganizationFilter { get; set; }
        public int? GroupFilter { get; set; }
        public int? RelationshipFilter { get; set; }
    }
}
