using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionBelk.Models
{
    public class Transaction
    {
        public int id { get; set;  } 

        public DateTime createdTimestamp { get; set; }
        public void setCreatedTimestamp(DateTime date) {
            createdTimestamp = date;
        }
        public string sender { get; set; }
        public string receiver { get; set;  }
        public string bankNumberSender { get; set; }
        public string emailSender { get; set; }
        public string phoneSender { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public byte[] approvalCertificate { get; set; }
        public int? TransactionStatusId { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public DateTime? approvalTimestamp { get; set; }
        public bool isDeleted { get; set; }

    }
}