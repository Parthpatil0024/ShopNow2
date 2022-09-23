using ShopNowBL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Repo;
using System.Net.Mail;
using System.Net;
using ShopNow2.ViewModels;
using System.Configuration;
using System.IO;
using System.Security.Policy;
using System.Data.OleDb;
using System.Data;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.Security;

namespace ShopNow2.Controllers
{
    
    public class HomeController : Controller
    {
        ExcepRepo ExcepRepo=new ExcepRepo();
        UserRepo userRepo=new UserRepo();
        StoreRepo storeRepo = new StoreRepo();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult authenticateUser(string EmailId,string Password)
        {
            bool result = false;
            var user = userRepo.authenticateUser(EmailId, Password);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.EmailId, false);

                    Session["User"] = user;
                Session["UserName"]=user.UserName;
                result = true;
                
            }
            else
            {
                ViewBag.InvalidUser = "InvalidUser";
                result = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["UserName"] = null;
            return RedirectToAction("Login");
        }
        
        public ActionResult forgotPassword()
        {
            return View();
        }
        
        public ActionResult VerifyEmail(string EmailId)
        {
            tblUser user = userRepo.VerifyEmail(EmailId);
            tblOTP objOTP = new tblOTP();
            bool result = false;
            if (user != null)
            {
                Random r = new Random();
                int otp = r.Next(100000, 999999);
                objOTP.OTP = Convert.ToString(otp);
                objOTP.Created_DateTime = DateTime.Now;
                objOTP.IsUsed = 0;
                objOTP.EmailId = user.EmailId;
                result = userRepo.SaveOTP(objOTP);

                if (result)
                {
                    try
                    {

                        var senderEmail = new MailAddress("prp9096@gmail.com", "ShopNow");
                        var receiverEmail = new MailAddress(user.EmailId, "Receiver");
                        var password = "vmfhxvhenhoarjgx";
                        var sub = "OTP for Password Reset";
                        var body = "Your OTP is " + objOTP.OTP + " valid for 5 minutes.";

                        MailMessage message = new MailMessage();
                        message.To.Add(user.EmailId);// Email-ID of Receiver  
                        message.Subject = sub;// Subject of Email  
                        message.From = senderEmail;// Email-ID of Sender  
                        message.IsBodyHtml = true;


                        message.Body = body;
                        SmtpClient SmtpMail = new SmtpClient();

                        SmtpMail.Host = "smtp.gmail.com";
                        SmtpMail.Port = 587;
                        SmtpMail.EnableSsl = true;
                        SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                        SmtpMail.UseDefaultCredentials = false;
                        SmtpMail.Credentials = new NetworkCredential(senderEmail.Address, password);
                        SmtpMail.Send(message);

                    }
                    catch (Exception ex)
                    {
                        ExcepRepo.addException(ex);
                    }
                }

            }

            return Json(user, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult VerifyOtp(string Otp, string EmailId)
        {
            tblOTP objOTP = userRepo.getOtpByEmail(EmailId);

            string result = "";


            if (objOTP.IsUsed == 1)
            {
                result = "OTP Already Used";
            }
            else
            {

                TimeSpan ts = DateTime.Now - objOTP.Created_DateTime;
                if (ts.Minutes <= 5)
                {
                    if (objOTP.OTP == Otp)
                    {
                        objOTP.IsUsed = 1;
                        userRepo.SaveOTP(objOTP);
                        result = "Valid OTP";
                    }
                    else
                    {
                        result = "Invalid OTP";
                    }
                }
                else
                {
                    result = "OTP expired";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult ResetPassword(string EmailId)
        {
            tblUser user = userRepo.VerifyEmail(EmailId);
            ViewBag.EmailId = user.EmailId;

            return PartialView("_ResetPassword");
        }
      
        public ActionResult SavePassword(string email, string pass1)
        {
            try
            {
                tblUser user = userRepo.VerifyEmail(email);
                user.Password = pass1;
                userRepo.addUser(user);


                return View();
            }
            catch (Exception ex) {
                ExcepRepo.addException(ex);
                return View("Error");
            }
            
        }

        public ActionResult RegisterAdmin()
        {
            UserAndStores userAndStores = new UserAndStores();
            userAndStores.lstStores = storeRepo.listStores();
            return View(userAndStores);
        }
        public ActionResult SaveAdmin(UserAndStores userAndStores)
        {
            try
            {
                var loggedInUser = (tblUser)HttpContext.Session["User"];
                userAndStores.user.CreatedBy = 17;
                userAndStores.user.RoleId = 1;

                bool result = userRepo.addUser(userAndStores.user);

                if (result)
                    return RedirectToAction("Login", "Home");

                return RedirectToAction("RegisterAdmin", "Home");
            }
            catch (Exception ex)
            {
                ExcepRepo.addException(ex);
                return View("Error");
            }
            
        }

        [Authorize(Roles = "Admin")]
        public ActionResult writeFile()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult saveTxt(string txtdata)
        {
            var line = Environment.NewLine;
            var fileName =  "logfile_" + @Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy") + ".txt";
            string filepath = ConfigurationManager.AppSettings["txtFilePath"].ToString()+fileName;
            try {
                if (!System.IO.File.Exists(filepath))
                {


                    System.IO.File.Create(filepath).Dispose();

                }
                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    string date = "Log Written Date:" + " " + DateTime.Now.ToString() + line ;
                    sw.WriteLine(date);
                    sw.WriteLine(txtdata);
                    sw.Flush();
                    sw.Close();


                }

                return RedirectToAction("Index", "Home");

            }
            catch (Exception e)
            {
                ExcepRepo.addException(e);

            }
            return RedirectToAction("Login", "Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult excelUpload()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UploadExcel()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    DataSet ds = new DataSet();
                    HttpPostedFileBase files = Request.Files[0];
                    string fileName = "ExcelUpload_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var path = ConfigurationManager.AppSettings["ExcelFilePath"].ToString();
                    var fullFilePath = path + "\\" + fileName;
                    files.SaveAs(path + "\\" + fileName);

                    string conString = string.Empty;

                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullFilePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;
                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(ds);
                                connExcel.Close();
                            }
                        }
                    }
                   string sqlconn = ConfigurationManager.ConnectionStrings["DBTContext"].ConnectionString;
                   var con = new SqlConnection(sqlconn);
                    SqlBulkCopy objbulk = new SqlBulkCopy(con);

                    objbulk.DestinationTableName = "tblCustomer";
                    //Mapping Table column      
                    objbulk.ColumnMappings.Add("CustomerName", "CustomerName");
                    objbulk.ColumnMappings.Add("MobileNo", "MobileNo");
                    objbulk.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    objbulk.ColumnMappings.Add("CreatedDate", "CreatedDate");
                    con.Open();
                    objbulk.WriteToServer(ds.Tables[0]);
                    con.Close();

                    return Json("success");
                }
                catch (Exception ex)
                {
                    ExcepRepo.addException(ex);
                }

            }
            return Json("error");

        }


        [Authorize(Roles = "Admin")]
        public ActionResult csvUpload()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UploadCsv()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    DataSet ds = new DataSet();
                    HttpPostedFileBase files = Request.Files[0];
                    string fileName = "ExcelUpload_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                    var path = ConfigurationManager.AppSettings["ExcelFilePath"].ToString();
                    var fullFilePath = path + @"\" + fileName;
                    files.SaveAs(path + @"\" + fileName);

                    string conString = string.Empty;

                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\'Text;HDR=YES;FMT=Delimited;\'";

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;
                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(ds);
                                connExcel.Close();
                            }
                        }
                    }
                    string sqlconn = ConfigurationManager.ConnectionStrings["DBTContext"].ConnectionString;
                    var con = new SqlConnection(sqlconn);
                    SqlBulkCopy objbulk = new SqlBulkCopy(con);

                    objbulk.DestinationTableName = "tblCustomer";
                    //Mapping Table column      
                    objbulk.ColumnMappings.Add("CustomerName", "CustomerName");
                    objbulk.ColumnMappings.Add("MobileNo", "MobileNo");
                    objbulk.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    objbulk.ColumnMappings.Add("CreatedDate", "CreatedDate");
                    con.Open();
                    objbulk.WriteToServer(ds.Tables[0]);
                    con.Close();

                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    ExcepRepo.addException(ex);
                    
                }

            }
            return Json("File Not Uploaded Successfully!");

        }



    }
}