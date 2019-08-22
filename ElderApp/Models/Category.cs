using System;
namespace ElderApp.Models
{
    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public string slug { get; set; }
        public Category()
        {
        }
    }
}
