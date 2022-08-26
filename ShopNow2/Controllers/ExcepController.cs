using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Models;
using ShopNowBL.Repo;

namespace ShopNow2.Controllers
{
    public class ExcepController
    {
        ExcepRepo excepRepo=new ExcepRepo();
        // GET: Excep
        public void addException(Exception ex)
        {
            tblErrorLog objerrorlog = new tblErrorLog();
           string exepurl = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

            objerrorlog.ExcepMsg = ex.Message.ToString();
            objerrorlog.ExcepType = ex.GetType().Name.ToString();
            objerrorlog.ExcepUrl = exepurl;
            objerrorlog.ExcepSource = ex.ToString();
            objerrorlog.LogDate = DateTime.Now;

            excepRepo.addException(objerrorlog);
           

        }
    }
}