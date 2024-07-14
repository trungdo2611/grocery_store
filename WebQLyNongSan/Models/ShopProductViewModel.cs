using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebQLyNongSan.Models
{
    public class ShopProductViewModel
    {
        public Product Products { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public int? StarAverage { get; set; }
    }
}