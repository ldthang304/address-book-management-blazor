using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBookManagement.Models
{
    [Table("Contacts")]
    public class Contact : Common
    {
        [Key]
        public int Id { get; set; }
        public string? Image { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NickName { get; set; }
        public string? JobTitle { get; set; }
        public string? PersonalEmail { get; set; }
        public string? WorkEmail { get; set; }
        public DateOnly? BirthDay { get; set; }
        public string? HomeAddress { get; set; }
        public string? WorkAddress { get; set; }

        //More Feature
        public bool? IsFavourite { get; set; }

        //Master Data
        public int? Gender { get; set; }
        public int? Relationship { get; set; }
        public int? Department { get; set; }
        public int? Group { get; set; }

        //Navigation properties
        public ICollection<Phone>? Phones { get; set; }
        public ICollection<Website>? Websites { get; set; }
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public ICollection<Task>? Tasks { get; set; }
        public ICollection<Note>? Notes { get; set; }

    }
}
