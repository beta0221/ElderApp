using System;
using SQLite;

namespace ElderApp.Models
{
    public class UserModel
    {
        [PrimaryKey,AutoIncrement]
        public int? Id { get; set; }
        public int User_id { get; set; }
        public string Id_code { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Wallet { get; set; }
        public int Rank { get; set; }
        public string Img { get; set; }
        public string Token { get; set; }

        public UserModel()
        {


        }
    }
}
