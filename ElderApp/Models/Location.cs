using System;
namespace ElderApp.Models
{
    public class Location
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string address { get; set; }
        public string link { get; set; }
        public object created_at { get; set; }
        public object updated_at { get; set; }

        private int _quantity;
        public int quantity {
            get {
                return _quantity;
            }
            set {
                _quantity = value;
                Qname = $"{name}(數量:{value})";
            }
        }

        public string Qname { get; set; }

        public string Lname {
            get
            {
                return $"名稱：{name}";
            }
        }
        public string Laddress
        {
            get
            {
                return $"地址：{address}";
            }
        }
    }
}
