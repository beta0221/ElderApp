using System;
using ElderApp.Services;

namespace ElderApp.Models
{
    public class Order
    {
        public int location_id { get; set; }
        public int receive { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string product { get; set; }
        public string img { get; set; }
        public string location { get; set; }
        public string address { get; set; }

        public bool Oreceive
        {
            get
            {
                if (receive == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public string Oimg
        {
            get
            {
                return $"{ApiServices.Host}{img}";
            }
        }

        public string Oproduct
        {
            get
            {
                return $"產品：{product}";
            }
        }

        public string Olocation
        {
            get
            {
                return $"據點：{location}";
            }
        }
        public string Oaddress
        {
            get
            {
                return $"地址：{address}";
            }
        }
        public string Ocreated_at
        {
            get
            {
                return $"兌換時間：{created_at}";
            }
        }
    }
}
