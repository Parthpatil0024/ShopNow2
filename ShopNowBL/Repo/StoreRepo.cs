using ShopNowBL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Data.Entity.Migrations;

namespace ShopNowBL.Repo
{
    public class StoreRepo
    {
        public List<tblStore> listStores()
        {
            List<tblStore> lst = new List<tblStore>();
            string conStr = ConfigurationManager.ConnectionStrings["DBTContext"].ConnectionString;

            SqlConnection Conn = new SqlConnection(conStr);
            string query = "SELECT * FROM tblStore";
            SqlCommand selectCMD = new SqlCommand(query, Conn);
           
            SqlDataAdapter getStores = new SqlDataAdapter();
            getStores.SelectCommand = selectCMD;
            
            try
            {
                DataSet stores = new DataSet();
                getStores.Fill(stores, "Stores");
               
                foreach (DataRow dr in stores.Tables["Stores"].Rows)
                {
                    tblStore store = new tblStore();

                    store.Id = Convert.ToInt32(dr["Id"]);
                    store.StoreName = Convert.ToString(dr["StoreName"]);
                    store.Address = Convert.ToString(dr["Address"]);
                    store.ContactNo = Convert.ToString(dr["ContactNo"]);
                    store.StartedDate = Convert.ToDateTime(dr["StartedDate"]);
                    store.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
                    store.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                    store.City = Convert.ToString(dr["City"]);
                    lst.Add(store);
                }

               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return lst;
        }

        public bool saveStore(tblStore newStore)
        {
            bool result = false;
            using (DBTContext context = new DBTContext())
            { 
                context.tblStores.AddOrUpdate(newStore);
                context.SaveChanges();
                result = true;
            }
            return result;
        }

        public tblStore findStoreById(int id)
        {
            tblStore store;
            using (DBTContext context = new DBTContext())
            {
                store = context.tblStores.Where(x => x.Id == id).FirstOrDefault();
            }
            return store;
        }

        public bool deleteStore(int id)
        {
            bool result = false;
            using (DBTContext context = new DBTContext())
            {
                tblStore store = context.tblStores.Where(x => x.Id == id).FirstOrDefault();
                context.tblStores.Remove(store);
                context.SaveChanges();
                result = true;
            }
            return result;
        }
    }
}
