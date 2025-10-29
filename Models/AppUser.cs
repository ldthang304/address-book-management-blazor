using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("AppUsers")]
    public class AppUser : Common
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }

        //Navigation properties
        public ICollection<Contact>? Contacts { get; set; }
    }
}
