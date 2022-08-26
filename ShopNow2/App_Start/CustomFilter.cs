using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow2 { 
    public class CustomFilter:ActionFilterAttribute
    {
  
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionUser = filterContext.HttpContext.Session["User"];
            var routeValues = filterContext.RequestContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RequestContext.RouteData.Values["action"].ToString();
            if (sessionUser == null && !routeValues.Equals("Home") )
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
            
                
               
            
            base.OnActionExecuting(filterContext);
        }
    }

}

