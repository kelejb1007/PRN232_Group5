using DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs.Admin.Orders
{
    public class OrderUpdateStatusDto
    {
        [Required]
        public OrderStatus NewStatus { get; set; }
    }
}
