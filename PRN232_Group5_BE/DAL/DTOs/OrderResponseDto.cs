using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs
{
    
        public class OrderResponseDto
        {
            public int OrderId { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }

