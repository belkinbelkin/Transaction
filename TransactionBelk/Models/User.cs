using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionBelk.Models
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int permission { get; set; }
        public DateTime? createdTimestamp { get; set; }
        public bool isDeleted { get; set; }

        public User() { }

        public User(string login, string password) {
            this.login = login;
            this.password = password;
        }
    }
}