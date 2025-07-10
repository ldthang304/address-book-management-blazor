
using AddressBookManagement.Commons.Enums;

namespace AddressBookManagement.Models
{
    public abstract class Common
    {
        public DeleteStatus? DeleteFlag { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public RecordVersion? RecordVersion { get; set; }
    }
}
