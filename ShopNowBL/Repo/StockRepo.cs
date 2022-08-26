using ShopNowBL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopNowBL.Repo
{
    public class StockRepo
    {
        public List<tblStock> listProducts()
        {
            List<tblStock> lstProducts;
            using(DBTContext context=new DBTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                lstProducts = context.tblStocks.ToList();
            }
            return lstProducts;
        }
        public bool saveProduct(tblStock newProduct)
        {
            bool result = false;
            using (DBTContext context = new DBTContext())
            {
               
                context.tblStocks.AddOrUpdate(newProduct);
                context.SaveChanges();
                result = true;
            }
            return result;

        }
        public tblStock getProductById(int id)
        {
            tblStock product;

            using (DBTContext context = new DBTContext())
            {

                context.Configuration.LazyLoadingEnabled = false;
                product = context.tblStocks.Where(x=>x.Id==id).SingleOrDefault();
            }
            return product;
        }
        public bool deleteProduct(int id)
        {
            bool result = false;
            using (DBTContext context = new DBTContext())
            {
                tblStock product = context.tblStocks.Where(x => x.Id == id).SingleOrDefault();
                context.tblStocks.Remove(product);
                context.SaveChanges();
                result = true;
            }
            return result;
        }

    }
}
