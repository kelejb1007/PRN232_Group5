
    using System;
    using System.Collections.Generic;

    namespace Intelligence_Book_WEB.Models
    {
        public class OrderDetailViewModel
        {
            public int OrderId { get; set; }
            public DateTime? OrderDate { get; set; }
            public string Status { get; set; } = "";
            public string? ShippingAddress { get; set; }
            public decimal TotalAmount { get; set; }

            public List<OrderDetailItemViewModel> Items { get; set; } = new();
        }


    }

