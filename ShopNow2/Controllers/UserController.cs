using ShopNowBL.Models;
using ShopNowBL.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNow2.ViewModels;


namespace ShopNow2.Controllers
{
    public class UserController : Controller
    {
        ExcepController excepController=new ExcepController();
        UserRepo userRepo=new UserRepo();
        StoreRepo storeRepo=new StoreRepo();
        // GET: User

        public ActionResult listUsers()
        {
            var lstUsers = userRepo.listUsers();
            return View(lstUsers);
        }


        public ActionResult UserProfile()
        {
            UserAndStores userAndStores = new UserAndStores();
            var loggedInUser = (tblUser)HttpContext.Session["User"];
            userAndStores.lstStores = storeRepo.listStores();
           userAndStores.user= userRepo.getUserById(loggedInUser.Id);

            
            return View(userAndStores);
        }


        public ActionResult addUser()
        {
            UserAndStores userAndStores = new UserAndStores();
            userAndStores.lstStores = storeRepo.listStores();
            return View(userAndStores);
        }
        public ActionResult saveUser(UserAndStores userAndStores)
        {
            try {

                var loggedInUser = (tblUser)HttpContext.Session["User"];
                userAndStores.user.CreatedBy = loggedInUser.Id;
                bool result = userRepo.addUser(userAndStores.user);
                if (result)
                {
                    return RedirectToAction("listUsers");
                }
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                excepController.addException(ex);
                return View("Error");
            }
           
        }

        public ActionResult editUser(int id)
        {
            UserAndStores userAndStores = new UserAndStores();
            userAndStores.user = userRepo.getUserById(id);
            userAndStores.lstStores = storeRepo.listStores();

            return View(userAndStores);
        }
       
        public ActionResult saveUserAfterEdit(UserAndStores userAndStores)
        {
            try
            {
                bool result = userRepo.saveUserAfterEdit(userAndStores.user);
                if (result)
                    return RedirectToAction("listUsers");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                excepController.addException(ex);
                return View("Error");
            }



        }
       

        public ActionResult deleteUser(int id)
        {
            bool result = userRepo.deleteUser(id);
            if (result)
            {
                return RedirectToAction("listUsers");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult listCashiers()
        {
            var lstUsers = userRepo.listUsers();
            var lstCashiers = from user in lstUsers
                                        where user.RoleId == 2
                                        select user;



            return View(lstCashiers);
        }

        public ActionResult listAdmins()
        {
            var lstUsers = userRepo.listUsers();
            var lstAdmins= from user in lstUsers
                              where user.RoleId == 1
                              select user;



            return View(lstAdmins);
        }
    }
}