using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNowBL.Models;
using ShopNowBL.Repo;

namespace ShopNow2.Controllers
{

   
    public class StoreController : Controller
    {
        ExcepRepo ExcepRepo=new ExcepRepo();
       StoreRepo storeRepo=new StoreRepo();
        // GET: Store
        public ActionResult listStores()
        {
            List<tblStore> listStores = new List<tblStore>();
            listStores=storeRepo.listStores();
            return View(listStores);
        }



        public ActionResult addStore()
        {
            
          tblStore newStore = new tblStore();

          return View(newStore);
            
        }

        public ActionResult saveStore(tblStore newStore)
        {
            try
            {
                var loggedInUser = (tblUser)HttpContext.Session["User"];
                newStore.CreatedBy = loggedInUser.Id;
                newStore.CreatedDate = DateTime.Now;
                bool result = storeRepo.saveStore(newStore);
                if (result)
                {
                    return RedirectToAction("listStores");
                }
                return RedirectToAction("addStore");
            }catch(Exception ex)
            {

                ExcepRepo.addException(ex);
                return View("Error");
            }
           
        }

        public ActionResult editStore(int Id)
        {
            tblStore store = storeRepo.findStoreById(Id);

            return View(store);

        }

        public  ActionResult saveStoreAfterEdit(tblStore objStore)
        {
            try {
                bool result = storeRepo.saveStore(objStore);
                if (result)
                {
                    return RedirectToAction("listStores");
                }
                return RedirectToAction("editStore");
            }catch(Exception ex)
            {
                ExcepRepo.addException(ex);
                return View("Error");
            }
          
        }

        public ActionResult deleteStore(int id)
        {
            bool result = storeRepo.deleteStore(id);
            if (result)
            {
                return RedirectToAction("listStores");
            }
            return RedirectToAction("Home", "Index");
        }
    }
}