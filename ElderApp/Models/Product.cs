using System;
namespace ElderApp.Models
{
    public class Product
    {
        public int id { get; set; }
        public int product_category_id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int price { get; set; }
        public string img { get; set; }
        public string info { get; set; }

        public string Image_Url
        {
            get
            {
                if (String.IsNullOrEmpty(img))
                {
                    return "event_default.png";
                }
                else
                {
                    return $"https://www.happybi.com.tw/images/products/{slug}/{img}";
                }
            }
        }

        public string Pname
        {
            get
            {
                return $"商品：{name}";
            }
        }

        public string Pprice
        {
            get
            {
                return $"樂幣：{price}";
            }
        }


    }
}
