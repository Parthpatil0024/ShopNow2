using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNowBL.Models;
using System.Data.Entity.Migrations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ShopNowBL.Repo
{
    public class TransactionRepo
    {
        StockRepo stockRepo = new StockRepo();
        public string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }


        public string CaptureTransaction(tblCustomer objCust, tblTransaction objTrans, List<tblTransactionItem> TransactionItems)
        {
            bool result = false;


            using (DBTContext context = new DBTContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        result = context.tblCustomers.Where(x => x.MobileNo == objCust.MobileNo).Any();

                        if (!result)
                        {
                            //Add Customer
                            tblCustomer customer = new tblCustomer();

                            customer.MobileNo = objCust.MobileNo;
                            customer.CustomerName = objCust.CustomerName;
                            customer.CreatedDate = DateTime.Now;
                            customer.CreatedBy = 1;

                            context.tblCustomers.Add(customer);
                            context.SaveChanges();
                            objTrans.CustomerId = customer.Id;

                        }
                        else
                        {

                            tblCustomer customer = context.tblCustomers.Where(x => x.MobileNo == objCust.MobileNo).FirstOrDefault();
                            objTrans.CustomerId = customer.Id;
                        }
                        //Capture Transaction
                        objTrans.CreatedDate = DateTime.Now;
                        objTrans.CreatedBy = 1;
                        objTrans.InvoiceNo = GenerateId();
                        objTrans.InvoiceDate = DateTime.Now;
                        context.tblTransactions.Add(objTrans);
                        context.SaveChanges();

                        //Capture Transaction Items
                        foreach (tblTransactionItem T in TransactionItems)
                        {
                            tblTransactionItem Item = new tblTransactionItem();
                            Item.InvoiceId = objTrans.InvoiceNo;
                            Item.Qty = T.Qty;
                            Item.ProductId = T.ProductId;
                            Item.Price = T.Price;
                            context.tblTransactionItems.Add(Item);
                            context.SaveChanges();


                            //Update Stock
                            tblStock stock = stockRepo.getProductById(Item.ProductId);
                            stock.ProductQty -= Convert.ToInt32(Item.Qty);
                            context.tblStocks.AddOrUpdate(stock);
                            context.SaveChanges();


                        }

                        transaction.Commit();
                        result = true;








                    }


                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = false;
                        throw ex;

                    }


                }
            }


            return objTrans.InvoiceNo;
        }


        public List<tblTransactionItem> getTItemsByInvoice(string InvoiceNo)
        {

              List<tblTransactionItem> listTransItems=new List<tblTransactionItem>();
               string conStr = ConfigurationManager.ConnectionStrings["DBTContext"].ConnectionString;

               SqlConnection Conn = new SqlConnection(conStr);

               SqlCommand selectCMD = new SqlCommand("SelectTItems", Conn);
               selectCMD.CommandType = CommandType.StoredProcedure;
               selectCMD.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);

               SqlDataAdapter getTransItems = new SqlDataAdapter();
               getTransItems.SelectCommand = selectCMD;
               DataSet ds = new DataSet();
               getTransItems.Fill(ds);

               foreach (DataRow dr in ds.Tables[1].Rows)
               {
                   tblTransactionItem transItem=new tblTransactionItem();
                   transItem.Id= Convert.ToInt32(dr["Id"]);
                   transItem.InvoiceId=Convert.ToString(dr["InvoiceId"]);
                   transItem.Price=Convert.ToDecimal(dr["Price"]);
                   transItem.ProductId= Convert.ToInt32(dr["ProductId"]);
                   transItem.Qty= Convert.ToInt32(dr["Qty"]);
                
            foreach (DataRow stock in ds.Tables[0].Rows)
               {
                   tblStock objProduct=new tblStock();
                    objProduct.Id= Convert.ToInt32(stock["Id"]);
                    objProduct.ProductName=Convert.ToString(stock["ProductName"]);
                   objProduct.BasePrice=Convert.ToDecimal(stock["BasePrice"]);
                   objProduct.SellingPrice= Convert.ToDecimal(stock["SellingPrice"]);
                    objProduct.Discount= Convert.ToInt32(stock["Discount"]);
                    objProduct.ProductQty= Convert.ToInt32(stock["ProductQty"]);
                    objProduct.CreatedBy= Convert.ToInt32(stock["CreatedBy"]);
                    objProduct.CreatedDate = Convert.ToDateTime(stock["CreatedDate"]);
                    transItem.tblStock=objProduct;
               }
                   listTransItems.Add(transItem);
               }
             
                 

            

            return listTransItems;
        }

        public tblTransaction getTransByInvoice(string InvoiceNo)
        {
            tblTransaction objTrans;
            using (DBTContext context = new DBTContext())
            {
                objTrans = context.tblTransactions.Where(x => x.InvoiceNo == InvoiceNo).FirstOrDefault();
            }
            return objTrans;
        }
    }
}
