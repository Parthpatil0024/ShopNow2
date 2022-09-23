using ShopNowBL.Models;
using ShopNowBL.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNow2.ViewModels;
using System.Threading.Tasks;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.ComponentModel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Net.Http;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ShopNow2.Controllers
{
    public class UserController : Controller
    {
        ExcepRepo ExcepRepo=new ExcepRepo();
        UserRepo userRepo=new UserRepo();
        StoreRepo storeRepo=new StoreRepo();
        StockRepo stockRepo=new StockRepo();
        // GET: User

        public ActionResult listUsers()
        {
            var lstUsers = Task.Run(async () => await userRepo.listUsers()).Result;
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
                ExcepRepo.addException(ex);
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
                ExcepRepo.addException(ex);
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
            var lstUsers = Task.Run(async () => await userRepo.listUsers()).Result;
            var lstCashiers = from user in lstUsers
                                        where user.RoleId == 2
                                        select user;



            return View(lstCashiers);
        }

        public ActionResult listAdmins()
        {
            var lstUsers = Task.Run(async () => await userRepo.listUsers()).Result;
            var lstAdmins= from user in lstUsers
                              where user.RoleId == 1
                              select user;



            return View(lstAdmins);
        }

        public FileResult exportToExcel(string type)
        {

            DataTable dt = new DataTable("Grid");
            PropertyDescriptorCollection properties;

            switch (type)
            {
                case "tblUser":
                    var lstUsers = Task.Run(async () => await userRepo.listUsers()).Result;

                     properties = TypeDescriptor.GetProperties(typeof(tblUser));
                    foreach (PropertyDescriptor prop in properties)
                    {
                       
                            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        
                    }
                    foreach (var user in lstUsers)
                    {
                        var values = new object[properties.Count];
                        for (int i = 0; i < properties.Count; i++)
                        {
                            //inserting property values to datatable rows
                            values[i] = properties[i].GetValue(user);
                        }
                        dt.Rows.Add(values);
                    }
                    break;

                case "tblStock":
                    var lstStock = stockRepo.listProducts();

                    properties = TypeDescriptor.GetProperties(typeof(tblStock));
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (!prop.Name.Equals("tblStock") && !prop.Name.Equals("tblTransactionItems"))
                        {
                            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                            //dt.Columns.Remove("tblTransactionItems");
                        }
                    }
                    foreach (var product in lstStock)
                    {
                        DataRow dr=dt.NewRow();
                        dr["Id"] = product.Id;
                        dr["ProductName"] = product.ProductName;
                        dr["ProductQty"] =product.ProductQty;
                        dr["SellingPrice"] = product.SellingPrice;
                        dr["BasePrice"] = product.BasePrice;
                        dr["Discount"] = product.Discount;
                        dr["CreatedBy"] = product.CreatedBy;
                        dr["CreatedDate"] = product.CreatedDate;
                       
                        


                        dt.Rows.Add(dr);
                        
                    }
                    break;

                case "tblStore":
                    var lstStore =storeRepo.listStores();

                    properties = TypeDescriptor.GetProperties(typeof(tblStore));
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (!prop.Name.Equals("tblStore"))
                        {
                            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                            
                        }
                    }
                    foreach (var store in lstStore)
                    {
                        var values = new object[properties.Count];
                        for (int i = 0; i < properties.Count; i++)
                        {
                            //inserting property values to datatable rows
                            values[i] = properties[i].GetValue(store);
                        }
                        dt.Rows.Add(values);
                    }
                    break;


            }











            using (XLWorkbook wb = new XLWorkbook())
            {
                string filename = type+ "List-" + DateTime.Now + ".xlsx";
                wb.Worksheets.Add(dt);
                
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
            }
        }
        public FileResult exportToCsv()
        {
            tblUser objUser = new tblUser();
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[9] { new DataColumn("Id"),
                                            new DataColumn("UserName"),
                                            new DataColumn("EmailId"),
                                            new DataColumn("MobileNo"),
                                            new DataColumn("RoleId"),

                                            new DataColumn("City"),
                                            new DataColumn("StoreId"),
                                            new DataColumn("CreatedBy"),
                                            new DataColumn("CreatedDate") });

            var lstUsers = Task.Run(async () => await userRepo.listUsers()).Result;

            foreach (var user in lstUsers)
            {
                dt.Rows.Add(user.Id, user.UserName, user.EmailId, user.MobileNo,
                    user.RoleId, user.City, user.StoreId, user.CreatedBy, user.CreatedDate);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                string filename = "UsersList-" + DateTime.Now + ".csv";
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
            }



        }
        /*-------------Api Call Methods--------------------*/



        public ActionResult lstUserApi()
        {
            IEnumerable<tblUser> listUsers = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44357/api/");
                //HTTP GET
                if (HttpContext.Session["jwt"] != null) { 
                string token = Convert.ToString(HttpContext.Session["jwt"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",JsonConvert.DeserializeObject<string>(token));
                }
                var responseTask = client.GetAsync("User/Get");

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<tblUser>>();
                    readTask.Wait();

                    listUsers = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    listUsers = Enumerable.Empty<tblUser>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(listUsers);
        }

        public ActionResult addUserApi()
        {
            UserAndStores userAndStores = new UserAndStores();
            userAndStores.lstStores = storeRepo.listStores();
            return View(userAndStores);
        }

        public ActionResult saveUserApi(UserAndStores userAndStores)
        {
            var loggedInUser = (tblUser)HttpContext.Session["User"];
            userAndStores.user.CreatedBy = loggedInUser.Id;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44357/api/");

                //HTTP POST
                if (HttpContext.Session["jwt"] != null)
                {
                    string token = Convert.ToString(HttpContext.Session["jwt"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var postTask = client.PostAsJsonAsync<tblUser>("User/Post", userAndStores.user);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("lstUserApi");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return RedirectToAction("addUserApi");
        }

        public ActionResult editUserApi(int Id)
        {
            UserAndStores userAndStores = new UserAndStores();
            userAndStores.lstStores=storeRepo.listStores();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44357/api/");
                //HTTP GET
                if (HttpContext.Session["jwt"] != null)
                {
                    string token = Convert.ToString(HttpContext.Session["jwt"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var responseTask = client.GetAsync("User/Get/" + Id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<tblUser>();
                    readTask.Wait();

                    userAndStores.user = readTask.Result;
                }
            }
            return View(userAndStores);
        }

        public ActionResult saveUserAfterEditApi(UserAndStores userAndStores)
        {


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44357/api/");

                //HTTP POST
                if (HttpContext.Session["jwt"] != null)
                {
                    string token = Convert.ToString(HttpContext.Session["jwt"]);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(token));
                }
                var putTask = client.PutAsJsonAsync<tblUser>("User/Put", userAndStores.user);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("lstUserApi");
                }
            }
            return View("editUserApi");
        }

        public ActionResult deleteUserApi(int Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44357/api/");

                //HTTP DELETE
                if (HttpContext.Session["jwt"] != null) { 
                string token = Convert.ToString(HttpContext.Session["jwt"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",JsonConvert.DeserializeObject<string>(token));
                }
                var deleteTask = client.DeleteAsync("User/Delete/" + Id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("lstUserApi");
                }
            }

            return RedirectToAction("Index","Home");
        }


        public ActionResult LoginApi()
        {
            return View();
        }

        public ActionResult authenticateUserApi(string EmailId,string Password)
        {
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44357/api/");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<string>("User/authenticate?EmailId="+EmailId+"&Password="+Password, null);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Session["jwt"] = result.Content.ReadAsStringAsync().Result;
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return Json(false, JsonRequestBehavior.AllowGet);
        }

    }



    }
