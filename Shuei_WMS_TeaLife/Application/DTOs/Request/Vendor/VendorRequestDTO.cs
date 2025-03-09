
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Vendor
{
    public class VendorRequestDTO
    {
        [Key] public int Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorImage { get; set; } // Handle file upload appropriately
        public string Abbreviation { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public EnumStatus Status { get; set; }
        public string Remarks { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string ShippingAddress { get; set; }
    }
}
