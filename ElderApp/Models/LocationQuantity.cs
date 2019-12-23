using System;
namespace ElderApp.Models
{
    public class LocationQuantity
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int location_id { get; set; }
        public int quantity { get; set; }
        public object created_at { get; set; }
        public object updated_at { get; set; }
    }
}
