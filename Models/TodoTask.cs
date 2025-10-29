using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("TodoTasks")]
    public class TodoTask : Common
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Type { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }
        public bool IsCompleted { get; set; } = false;

        //Navigation properties
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}
