using ShopNow.ViewModels;
using ShopNowBL.Models;
using ShopNowBL.Repo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Rotativa;

namespace ShopNow2.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        ExcepController excepController=new ExcepController();
        TransactionRepo transactionRepo = new TransactionRepo();
        UserRepo userRepo = new UserRepo();
       

        public ActionResult SelectProducts()
        {
            return View();
        }

        public ActionResult ajaxAddTransaction(string CustomerName, string MobileNo, int TotalQty, decimal InvoiceAmount, decimal GST, decimal TotalDiscount, string PaymentMethod, List<tblTransactionItem> TransactionItems)
        {
            try
            {
                tblCustomer customer = new tblCustomer();
                customer.CustomerName = CustomerName;
                customer.MobileNo = MobileNo;

                tblTransaction transaction = new tblTransaction();
                transaction.TotalQty = TotalQty;
                transaction.InvoiceAmount = InvoiceAmount;

                transaction.GST = GST;
                transaction.TotalDiscount = TotalDiscount;
                transaction.PaymentMethod = PaymentMethod;

                var result = transactionRepo.CaptureTransaction(customer, transaction, TransactionItems);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { 
            excepController.addException(ex);
                return View("Error");
            }
           
        }

        public ActionResult generateInvoice(string InvoiceNo,string hide, string Email)
        {
            InvoiceViewModel invoiceModel = new InvoiceViewModel();
            invoiceModel.listTransItems = transactionRepo.getTItemsByInvoice(InvoiceNo);
            invoiceModel.objTrans = transactionRepo.getTransByInvoice(InvoiceNo);
            invoiceModel.customer = userRepo.findCustomerById(invoiceModel.objTrans.CustomerId);
            ViewBag.Hide = hide;
            ViewBag.Email = Email;


            return PartialView("_generateInvoice", invoiceModel);
        }


        public ActionResult PrintInvoice(string InvoiceNo, string Email)
        {

            var a = new ViewAsPdf();
            a.ViewName = "_generateInvoice";
            
            InvoiceViewModel invoiceModel = new InvoiceViewModel();

            invoiceModel.listTransItems = transactionRepo.getTItemsByInvoice(InvoiceNo);
            invoiceModel.objTrans = transactionRepo.getTransByInvoice(InvoiceNo);
            invoiceModel.customer = userRepo.findCustomerById(invoiceModel.objTrans.CustomerId);


            ViewBag.Hide = 1;

            a.Model = invoiceModel;
            var pdfBytes = a.BuildFile(this.ControllerContext);

            // Optionally save the PDF to server in a proper IIS location.
            var fileName = invoiceModel.customer.CustomerName + "-" + invoiceModel.objTrans.InvoiceNo + ".pdf";
            //path for storing pdf
            var path = Server.MapPath("~/Temp/" + fileName);

            System.IO.File.WriteAllBytes(path, pdfBytes);

            MemoryStream ms = new MemoryStream(pdfBytes);

            try
            {

                var senderEmail = new MailAddress("prp9096@gmail.com", "ShopNow");
                var receiverEmail = new MailAddress(Email, "Receiver");
                var password = "ydsipwogcmhnujip";
                var sub = "Invoice For Your Purchase dated " + invoiceModel.objTrans.InvoiceDate;
                var body = "Thank you for your purchase " + invoiceModel.customer.CustomerName + ". Your Invoice is here. Visit Again!!";

                MailMessage message = new MailMessage();
                message.To.Add(Email);// Email-ID of Receiver  
                message.Subject = sub;// Subject of Email  
                message.From = senderEmail;// Email-ID of Sender  
                message.IsBodyHtml = true;
                Attachment data = new Attachment(ms, fileName, "application/pdf");
                message.Attachments.Add(data);
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
                excepController.addException(ex);
                return View("Error");
            }





            return RedirectToAction("SelectProducts", "Transaction");
        }



    }
}