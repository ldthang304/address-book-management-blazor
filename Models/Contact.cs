using Microsoft.Extensions.Options;
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
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        
        public string? FirstName { get; set; }
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Required(ErrorMessage = "Last name is required")]
        public string? LastName { get; set; }
        [StringLength(30, ErrorMessage = "Nick name cannot exceed 30 characters")]
        public string? NickName { get; set; }
        [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters")]
        public string? JobTitle { get; set; }
        [EmailAddress(ErrorMessage = "Personal email is not valid")]
        [StringLength(100, ErrorMessage = "Personal email cannot exceed 100 characters")]

        public string? PersonalEmail { get; set; }
        [EmailAddress(ErrorMessage = "Work email is not valid")]
        [StringLength(100, ErrorMessage = "Work email cannot exceed 100 characters")]
        public string? WorkEmail { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="Birth Day is required")]
        public DateOnly? BirthDay { get; set; }
        [StringLength(200, ErrorMessage = "Home address cannot exceed 200 characters")]
        public string? HomeAddress { get; set; }
        [StringLength(200, ErrorMessage = "Work address cannot exceed 200 characters")]
        public string? WorkAddress { get; set; }

        //More Feature
        public bool? IsFavourite { get; set; }

        //Master Data
        public int? Gender { get; set; }
        public int? Relationship { get; set; }
        public int? Department { get; set; }
        public int? Group { get; set; }

        //Navigation properties
        [ValidateObjectMembers]
        public IList<Phone>? Phones { get; set; }
        [ValidateObjectMembers]
        public IList<Website>? Websites { get; set; }
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        [ValidateObjectMembers]
        public IList<TodoTask>? Tasks { get; set; }
        [ValidateObjectMembers]
        public IList<Note>? Notes { get; set; }

    }
}
