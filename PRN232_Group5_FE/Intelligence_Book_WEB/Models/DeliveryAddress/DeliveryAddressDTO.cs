using System.ComponentModel.DataAnnotations;

namespace Intelligence_Book_WEB.Models.DeliveryAddress
{
    public class DeliveryAddressDTO
    {
        public int AddressId { get; set; }
        
        [Required(ErrorMessage = "Receiver name is required")]
        [StringLength(100, ErrorMessage = "Keep receiver name under 100 characters")]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Keep phone number under 20 characters")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Keep address under 500 characters")]
        public string AddressLine { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
    }
}
