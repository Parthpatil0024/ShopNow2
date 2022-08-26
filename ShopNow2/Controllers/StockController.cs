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
        ExcepController excepController = new ExcepController();

        StockRepo stockRepo =new StockRepo();
        // GET: Stock
        public ActionResult listProducts()
        {
            List<tblStock> lstProducts;
            
             lstProducts = stockRepo.listProducts();

           

            return View(lstProducts);
        }
        public ActionResult addProduct()
        {
           
                tblStock newProduct = new tblStock();
                return View(newProduct);
            
            
        }
        public ActionResult saveProduct(tblStock newProduct)
        {
            /* var loggedInUser = (tblUser)HttpContext.Session["User"];
             newProduct.CreatedBy = loggedInUser.Id;
             newProduct.CreatedDate = DateTime.Now;
             bool result = stockRepo.saveProduct(newProduct);
             if (result)
             {
                 return RedirectToAction("listProducts");
             }
             return RedirectToAction("addProduct");*/

            /*try
            {*/
                var loggedInUser = (tblUser)HttpContext.Session["User"];
                newProduct.CreatedBy = loggedInUser.Id;
                newProduct.CreatedDate = DateTime.Now;
                bool result = stockRepo.saveProduct(newProduct);
                if (result)
                {
                    return RedirectToAction("listProducts");
                }
                return RedirectToAction("addProduct");
           /* }
            catch (Exception ex)
            { 
                excepController.addException(ex);
                return View("Error");
               
            }*/
        }

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
                excepController.addException(ex);
                return View("Error");

            }

        }
        public ActionResult editProduct(int id)
        {
            tblStock product = stockRepo.getProductById(id);
            return View(product);
        }
        public ActionResult deleteProduct(int id)
        {
            bool result = stockRepo.deleteProduct(id);
            if (result)
            {
                return RedirectToAction("listProducts");
            }
            return RedirectToAction("Home", "Index");
        }

        public ActionResult ajaxGetProductList()
        {
            var productList = stockRepo.listProducts();
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ajaxGetProdById(string Id)
        {
            tblStock product = stockRepo.getProductById(Convert.ToInt32(Id));
            return Json(product, JsonRequestBehavior.AllowGet);
        }
        

    }
}