using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNowBL.Models;
using System.Data.Entity.Migrations;

namespace ShopNow2.ViewModels
{
    public class UserAndStores
    {
        public tblUser user {get; set;}
        public List<tblStore> lstStores {get; set;}
    }
}
