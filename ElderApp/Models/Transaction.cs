using System;
namespace ElderApp.Models
{
    public class Transaction
    {


        public string Tran_id { get; set; }

        public int User_id { get; set; }

        public string Event { get; set; }

        public int Amount { get; set; }

        public string AmountString { get { return $"{Sign}{Amount}"; } }

        public string Text_color { get; set; }

        public string Sign { get; set; }

        private bool give_take;
        public bool Give_take
        {
            get { return give_take; }
            set { 
                give_take = value;
                if (value == true)
                {
                    Text_color = "Green";
                    Sign = "+";
                }
                else
                {
                    Text_color = "Red";
                    Sign = "-";
                }
            }
        }
        public string Target_name { get; set; }

        private string created_at;
        public string Created_at
        {
            get
            {
                return created_at.Substring(0,10);
            }
            set
            {
                created_at = value;
            }
        }

        public Transaction()
        {
        }
    }
}
