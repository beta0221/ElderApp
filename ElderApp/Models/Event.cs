using System;
namespace ElderApp.Models
{
    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        //public string image_url { get; set; }

        public string body { get; set; }

        private string _dateTime;
        public string dateTime
        {
            get
            {
                return $"活動時間:{_dateTime}";
            }
            set
            {
                _dateTime = value;
            }
        }

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

        

        private string _deadline;
        public string deadline
        {
            get
            {
                return $"報名截止日:{_deadline}";
            }
            set
            {
                _deadline = value;
            }
        }
        public string created_at { get; set; }
        public string slug { get; set; }


        public string location { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }

        public string catAndDic
        {
            get
            {
                return $"{category_name}-{district_name}";
            }
        }

        public int district_id { get; set; }
        public string district_name { get; set; }

        public string Image_Url
        {
            get
            {
                if (String.IsNullOrEmpty(image))
                {
                    return "event_default.png";
                }
                else
                {
                    return $"https://www.happybi.com.tw/images/events/{slug}/{image}";
                    
                }

            }
        }
        public int maximum { get; set; }
        public int numberOfPeople { get; set; }

        public string people {
            get
            {
                return $"人數: {numberOfPeople} / {maximum}";
            }
        }
        public Event()
        {
        }
    }
}