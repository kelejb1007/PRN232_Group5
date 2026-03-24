using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs
{
    public class CheckoutRequest
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; }
        public string ReceiverName { get; set; }
        public string PhoneNumber { get; set; }
        public string? CouponCode { get; set; }
    }
}
