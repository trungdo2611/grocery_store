using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebQLyNongSan.Models
{
    public class ShopViewModel1
    {
        public List<Product> _products { get; set; }
        public List<Category> _categories { get; set; }
        public Dictionary<int, decimal?> AverageRatings { get; set; }
    }
}