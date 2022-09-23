using ShopNowBL.Models;
using ShopNowBL.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow2.Controllers
{
    public class StockController : Controller
    {
        ExcepRepo ExcepRepo = new ExcepRepo();

        StockRepo stockRepo =new StockRepo();
        // GET: Stock
        [Authorize(Roles = "Admin,Cashier")]
        public ActionResult listProducts()
        {
            List<tblStock> lstProducts;
            
             lstProducts = stockRepo.listProducts();

           

            return View(lstProducts);
        }

        [Authorize(Roles = "Admin,Cashier")]

        public ActionResult addProduct()
        {
           
                tblStock newProduct = new tblStock();
                return View(newProduct);
            
            
        }

        [Authorize(Roles = "Admin,Cashier")]

        public ActionResult saveProduct(tblStock newProduct)
        {
            try
            {
                var loggedInUser = (tblUser)HttpContext.Session["User"];
                newProduct.CreatedBy = loggedInUser.Id;
                newProduct.CreatedDate = DateTime.Now;
                bool result = stockRepo.saveProduct(newProduct);
                if (result)
                {
                    return RedirectToAction("listProducts");
                }
                return RedirectToAction("addProduct");
            }

            catch (Exception ex)
            {
                ExcepRepo.addException(ex);
                return View("Error");

            }
        }
        [Authorize(Roles = "Admin,Cashier")]

        public ActionResult saveProductAfterEdit(tblStock product)
        {
            try {
                bool result = stockRepo.saveProduct(product);
                if (result)
                {
                    return RedirectToAction("listProducts");
                }
                return RedirectToAction("editProduct");
            }
            catch (Exception ex)
            {
                ExcepRepo.addException(ex);
                return View("Error");

            }

        }

        [Authorize(Roles = "Admin,Cashier")]

        public ActionResult editProduct(int id)
        {
            tblStock product = stockRepo.getProductById(id);
            return View(product);
        }

        [Authorize(Roles = "Admin")]

        public ActionResult deleteProduct(int id)
        {
            bool result = stockRepo.deleteProduct(id);
            if (result)
            {
                return RedirectToAction("listProducts");
            }
            return RedirectToAction("Home", "Index");
        }

        [Authorize(Roles = "Admin,Cashier")]

        public ActionResult ajaxGetProductList()
        {
            var productList = stockRepo.listProducts();
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Cashier")]

        public ActionResult ajaxGetProdById(string Id)
        {
            tblStock product = stockRepo.getProductById(Convert.ToInt32(Id));
            return Json(product, JsonRequestBehavior.AllowGet);
        }
        

    }
}