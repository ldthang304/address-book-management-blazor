using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("Organizations")]
    public class Organization : Common
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Websites { get; set; }

        //Master Data
        public int? Country { get; set; }
        public int? Industry { get; set; }
        public int? Type { get; set; }

        //Navigation properties
        public ICollection<Contact>? Contacts { get; set; }
    }
}
