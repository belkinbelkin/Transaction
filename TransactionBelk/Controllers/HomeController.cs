using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransactionBelk.Controllers;
using TransactionBelk.Models;

namespace TransactionBelk.Controllers
{
    public class HomeController : Controller
        
    {
        public HomeController() { }
        const int status_pending = 1;
        const int status_cancelled = 2;
        const int status_approved = 3;

        TransactionContext db = new TransactionContext();

        public ActionResult Index()
        {

            ViewBag.Hi = "Hello there! This is my test transaction application";

            return View();
        }

        public ActionResult Table() {
            return View();
        }

        [HttpGet]
        public ActionResult MakeTransaction() {
            return View(); 
        }

        [HttpPost]
        public ActionResult MakeTransaction(Transaction trans)
        {
            trans.setCreatedTimestamp(DateTime.Now);
            trans.approvalTimestamp = null;
            var status = db.statuses.Where(arg => arg.code == status_pending).FirstOrDefault();
            trans.TransactionStatusId = status.id;
            db.transactions.Add(trans);
            db.Entry(trans).State = EntityState.Added;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex) {
                return Content(ex.Message + ex.StackTrace + ex.InnerException);
            }
            return Redirect("/Home/Index");
        }

        [HttpGet]
        public ActionResult Login() {

            return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string password)
        {
            bool isPresent = db.users.Where(u => u.login == login && u.password == password).Any();

            if (!isPresent) {
                return Redirect("/Home/Login");
            } else {
                HttpContext.Response.Cookies["token"].Value = Base64Encode(password);
                HttpContext.Response.Cookies["user"].Value = Base64Encode(login);
                return Redirect("/Home/Transactions");
            }
        }

        public ActionResult Logout() {
            HttpContext.Response.Cookies["token"].Value = "";
            HttpContext.Response.Cookies["user"].Value = "";
            return Redirect("/Home/Login");
        }


        public ActionResult Transactions() {
            if (isAuthorized()) {
                ViewBag.editable = isEditor();
                ViewBag.isApprover = isApprover();
                ViewBag.statuses = db.statuses;
                //var transWithStatus = db.transactions.Include(tran => tran.TransactionStatus);
                ViewBag.transactions = db.transactions.Include(arg => arg.TransactionStatus).Include(arg=>arg.User).Where(arg => arg.isDeleted == false).Take(100).OrderByDescending(arg => arg.createdTimestamp).ToList();
                return View();
            }      else {
                return Redirect("/Home/Login");
            }
        }

        [HttpGet]
        public ActionResult EditTransaction(int? id)
        {
            if (id != null)
            {
                var toEdit = db.transactions.Include(to => to.TransactionStatus).Where(t => t.id == id && t.isDeleted == false).FirstOrDefault();
                if (isEditor() || isApprover())
                {
                    ViewBag.isApprover = isApprover();
                    return View(toEdit);
                }
                
            }
            return Redirect("Home/Transactions");
        }

        [HttpPost]
        public ActionResult UploadFile(int id, HttpPostedFileBase upload) {
            if (upload != null) {

                if(upload.ContentType == "image/png") { 

                var toEdit = db.transactions.Where(t => t.id == id).FirstOrDefault();
                byte[] cert = new byte[upload.ContentLength];
                // upload.InputStream.Read(avatar, 0, upload.ContentLength);
                toEdit.approvalCertificate = cert;
                db.Entry(toEdit).State = EntityState.Modified;
                db.SaveChanges();
                } else {
                    return Content("<script> alert(\"Error! File format should be .png \"); window.location.assign('/Home/EditTransaction/" + id.ToString() + "')</script>");
                }
            }
            return Redirect("/Home/EditTransaction/" + id);
        }

        [HttpPost]
        public ActionResult EditTransaction([Bind(Exclude = "TransactionStatusId, TransactionStatus")] Transaction t) {
            if (isEditor() || isApprover()) {
           //     t.approvalTimestamp = null;
                t.TransactionStatusId = db.statuses.Where(arg => arg.code == status_pending).FirstOrDefault().id;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Redirect("/Home/Transactions");
        }

        [HttpGet]
        public ActionResult DeleteTransaction(int id)
        {
            if (isEditor() || isApprover())
            {
                var toDelete = db.transactions.Where(t => t.id == id).FirstOrDefault();
                toDelete.isDeleted = true;
                db.Entry(toDelete).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Redirect("/Home/Transactions");
        }

        [HttpGet]
        public ActionResult ApproveTransaction(int id)
        {
            if (db.transactions.Where(t => t.id == id).FirstOrDefault().approvalCertificate != null)
            {
                return transStatusAux(id, status_approved);
            }
            else {
                return Content("<script>alert(\"Please upload the certificate!\"); window.location.assign('/Home/EditTransaction/" + id.ToString() + "')</script>");
            }
          
        }

        public FileResult GetFile(int id)
        {
            // Путь к файлу
            byte[] fileBytes = db.transactions.Where(t => t.id == id).FirstOrDefault().approvalCertificate;
            // Тип файла - content-type
            string file_type = "image/png";
            // Имя файла - необязательно
            string file_name = "certificate.png";
            return File(fileBytes, file_type, file_name);
        }

        [HttpGet]
        public ActionResult CancelTransaction(int id) {
            return transStatusAux(id, status_cancelled);
        }

        private ActionResult transStatusAux(int id, int status) {
            if (isApprover())
            {
                var toUpdate = db.transactions.Where(t => t.id == id).FirstOrDefault();
                toUpdate.TransactionStatusId = db.statuses.Where(arg => arg.code == status).FirstOrDefault().id;
                User fromCookies = getLoginUserToken();
                toUpdate.UserId = db.users.Where(user => user.login == fromCookies.login && user.password == fromCookies.password).First().id;
                toUpdate.approvalTimestamp = DateTime.Now;
                db.Entry(toUpdate).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Redirect("/Home/Transactions");
        }


        [HttpPost]
        public ActionResult FilterTransactions(string findBySender, 
            string findByRecipient, string findByBankNumber, string findByApprovedBy, 
            string findByEmail, string findByPhone, int findByStatus,
            DateTime? ApprDateFrom, DateTime? ApprDateTo,
            DateTime? CreateDateFrom, DateTime? CreateDateTo)
        {
            var transactions = db.transactions.Include(t=> t.TransactionStatus).Include(arg => arg.User).Where(t=> t.isDeleted==false).OrderByDescending(arg => arg.createdTimestamp).ToList();
            ViewBag.editable = isEditor();
            ViewBag.isApprover = isApprover();
            if (isNotNullOrEmpty(findBySender))
            {
               transactions = db.transactions.Where(a => a.sender.Contains(findBySender)).ToList();
         
            }
            if (isNotNullOrEmpty(findByRecipient))
            {
                transactions = db.transactions.Where(a => a.receiver.Contains(findByRecipient)).ToList();

            }

            if (isNotNullOrEmpty(findByBankNumber))
            {
                transactions = db.transactions.Where(a => a.bankNumberSender.Contains(findByBankNumber)).ToList();

            }

            if (isNotNullOrEmpty(findByApprovedBy))
            {
                transactions = db.transactions.Where(a => a.User.name.Contains(findByApprovedBy)).ToList();

            }

            if (isNotNullOrEmpty(findByEmail))
            {
                transactions = db.transactions.Where(a => a.emailSender.Contains(findByEmail)).ToList();

            }

            if (isNotNullOrEmpty(findByPhone))
            {
                transactions = db.transactions.Where(a => a.phoneSender.Contains(findByPhone)).ToList();
            }

            if (findByStatus != 0) {
                transactions = db.transactions.Where(a=> a.TransactionStatusId == findByStatus).ToList();
            }

            if (ApprDateFrom != null) {
                transactions = db.transactions.Where(a => a.approvalTimestamp>ApprDateFrom).ToList();
            }

            if (ApprDateTo != null)
            {
                transactions = db.transactions.Where(a => a.approvalTimestamp < ApprDateTo).ToList();
            }

            if (CreateDateFrom != null)
            {
                transactions = db.transactions.Where(a => a.createdTimestamp > CreateDateFrom).ToList();
            }

            if (CreateDateTo != null)
            {
                transactions = db.transactions.Where(a => a.createdTimestamp < CreateDateTo).ToList();
            }

            return PartialView(transactions);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public bool isAuthorized() {
            User fromCookies = getLoginUserToken();
            return db.users.Where(user =>user.login == fromCookies.login &&
            user.password == fromCookies.password).Any();
        }

        public bool isApprover() {

            return resolvePermission(3);
        }

        public bool isEditor()
        {
            //  return db.users.Where(u => permissionAux(u, 4, getLoginUserToken())).Any();
            return resolvePermission(2);
           
        }

        public bool isViewer()
        {
            return resolvePermission(1);
        }

        public bool resolvePermission(int status) {

            User fromCookies = getLoginUserToken();
            return db.users.Where(user => user.login == fromCookies.login &&
                               user.password == fromCookies.password &&
                               user.permission == status).Any();
        }

        //public Func<User, int, User, bool> permissionAux =
        //    (user, status, fromCookies) => user.login == fromCookies.login &&
        //                        user.password == fromCookies.password &&
        //                        user.permission == status;

        public User getLoginUserToken() {
            string passDecoded = Base64Decode(HttpContext.Request.Cookies["token"].Value);
            string login = Base64Decode(HttpContext.Request.Cookies["user"].Value);
            return new User(login, passDecoded);
        }

        public bool isNotNullOrEmpty(string s)
        {
            return (s != null && s != "");
        }

    }
}