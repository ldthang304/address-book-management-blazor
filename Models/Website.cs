using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("Websites")]
    public class Website : Common
    {
        [Key]
        public int Id { get; set; }
        public string? Url { get; set; }

        //Navigation properties
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
        public int? WebsiteTypeId { get; set; }
        public WebsiteType? WebsiteType { get; set; }
    }
}
