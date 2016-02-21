using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionBelk.Models
{
        public class StatusInit : System.Data.Entity.DropCreateDatabaseAlways<TransactionContext>
    {
            protected override void Seed(TransactionContext db)
            {
            db.statuses.Add(new TransactionStatus { code = 1, label = "Pending" });
            db.statuses.Add(new TransactionStatus { code = 2, label = "Cancelled" });
            db.statuses.Add(new TransactionStatus { code = 3, label = "Approved" });
            db.users.Add(new User { login = "root", password = "root", permission = 4, isDeleted = false, name = "root", createdTimestamp = DateTime.Now });
            base.Seed(db);
            }
        }
    
}