using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TransactionBelk.Models
{
    public class TransactionContext : DbContext
    {
        public TransactionContext()
            :base("TransactionContext")
        { }


        public DbSet<Transaction> transactions { get; set; }
        public DbSet<TransactionStatus> statuses { get; set; }
        public DbSet<User> users { get; set; }
    }
}