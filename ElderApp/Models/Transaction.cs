using System;
namespace ElderApp.Models
{
    public class Transaction
    {


        public string Tran_id { get; set; }

        public int User_id { get; set; }

        public string Event { get; set; }

        public int Amount { get; set; }

        public string Text_color { get; set; }

        private bool give_take;
        public bool Give_take {
            get { return give_take; }
            set { 
                give_take = value;
                if (value == true)
                {
                    Text_color = "Green";
                }
                else
                {
                    Text_color = "Red";
                }
            }
        }

        public string Created_at { get; set; }

        public Transaction()
        {
        }
    }
}
