using System;
using System.Collections.Generic;

namespace ElderApp.Models
{
    public class ProductAndCat
    {
        public List<Product> products { get; set; }
        public List<ProductCategory> productCategory { get; set; }

        public ProductAndCat()
        {
        }
    }
}
