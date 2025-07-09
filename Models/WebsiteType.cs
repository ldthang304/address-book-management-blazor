using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("WebsiteTypes")]
    public class WebsiteType
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        //Navigation properties
        public ICollection<Website>? Websites { get; set; }
    }
}
