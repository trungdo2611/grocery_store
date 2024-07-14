using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebQLyNongSan.Models
{
    public class OrderViewModel
    {
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int PaymentType { get; set; }
        public int PaymentTypeVN { get; set; }
    }
}