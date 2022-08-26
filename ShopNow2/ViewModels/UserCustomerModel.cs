using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNowBL.Models;

namespace ShopNow.ViewModels
{
    public class UserCustomerModel
    {
       public List<tblCustomer> listCustomer { get; set; }
       public List<tblUser> listUser { get; set; }
        
        public int UserCount { get; set; }
        public int CustomerCount { get; set; }
    }
}