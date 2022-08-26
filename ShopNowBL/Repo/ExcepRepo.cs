using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNowBL.Models;


namespace ShopNowBL.Repo
{
    public class ExcepRepo
    {
        public bool addException(tblErrorLog ex)
        {
            bool result = false;
            using(DBTContext context=new DBTContext())
            {
                context.tblErrorLogs.Add(ex);
                context.SaveChanges();
                result = true;
            }
            return result;
        }
    }
}
