using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("Phones")]
    public class Phone : Common
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Phone number is required", AllowEmptyStrings = false)]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Number { get; set; }

        //Navigation properties
        public int? PhoneType { get; set; }
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}
