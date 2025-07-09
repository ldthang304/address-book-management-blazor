using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("Masters")]
    public class Master
    {
        [Key]
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public int? TypeKey { get; set; }
        public string? TypeValue { get; set; }
    }
}
