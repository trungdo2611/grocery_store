using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebQLyNongSan.Models;

namespace WebQLyNongSan.Helper
{
    public class ProductViewModel
    {
        public Product Products { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
    }
}