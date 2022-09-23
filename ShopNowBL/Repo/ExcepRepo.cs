using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNowBL.Models;
using context= System.Web.HttpContext;


namespace ShopNowBL.Repo
{
    public class ExcepRepo
    {
        public void addException(Exception ex)
        {
            
            using(DBTContext context=new DBTContext())
            {
                tblErrorLog objerrorlog = new tblErrorLog();
                string exepurl = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                objerrorlog.ExcepMsg = ex.Message.ToString();   
                objerrorlog.ExcepType = ex.GetType().Name.ToString();
                objerrorlog.ExcepUrl = exepurl;
                objerrorlog.ExcepSource = ex.ToString();
                objerrorlog.LogDate = DateTime.Now;

                context.tblErrorLogs.Add(objerrorlog);
                context.SaveChanges();
               
            }
            
        }
    }
}
