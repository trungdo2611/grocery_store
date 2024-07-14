using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Documents;

namespace WebQLyNongSan.Models
{
    public class ProductDetailViewModel
    {
        public Product DetailsModel { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public int? StarAverage { get; set; }

    }

}