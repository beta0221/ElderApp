using System;
namespace ElderApp.Models
{
    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }

        //public string image_url { get; set; }

        public string body { get; set; }

        public string dateTime { get; set; }

        private bool participate;
        public bool Participate
        {
            get { return participate; }
            set
            {
                participate = value;
                if (value == true)
                {
                    participate_color = "Blue";
                    participate_text = "已參加";
                    btn_text = "取消參加";
                    btn_color = "Red";

                }
                else
                {
                    participate_color = "Black";
                    participate_text = "未參加";
                    btn_text = "參加";
                    btn_color = "Green";
                }

            }
        }
        public string participate_text { get; set; }
        public string participate_color { get; set; }
        public string btn_text { get; set; }
        public string btn_color { get; set; }

        public string location { get; set; }
        public string deadline { get; set; }
        public string created_at { get; set; }
        public string slug { get; set; }
        public int category_id { get; set; }

        public Event()
        {
        }
    }
}