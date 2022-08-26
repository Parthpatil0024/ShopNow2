using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNowBL.Models;

namespace ShopNow.ViewModels
{
    public class InvoiceViewModel
    {
       public  List<tblTransactionItem> listTransItems { get; set; }
        public tblTransaction objTrans { get; set; }
       public tblCustomer customer { get; set; }

    }
}