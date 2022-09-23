using ShopNowBL.Models;
using ShopNowBL.Repo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace ShopNow2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {

        ExcepRepo excepRepo=new ExcepRepo();
        // GET: Employee
        public ActionResult listEmployees()
        {
            try {
                List<EmployeeModel> lstEmp = new List<EmployeeModel>();
                DataSet ds = new DataSet();
                ds.ReadXml(Server.MapPath("~/Xml/EmployeeList.xml"));
                DataView dvPrograms;
                dvPrograms = ds.Tables[0].DefaultView;
                dvPrograms.Sort = "Id";
                foreach (DataRowView dr in dvPrograms)
                {
                    EmployeeModel objEmp = new EmployeeModel();
                    objEmp.Id = Convert.ToInt32(dr[0]);
                    objEmp.EmpName = Convert.ToString(dr[1]);
                    objEmp.Address = Convert.ToString(dr[2]);
                    objEmp.Salary = Convert.ToDecimal(dr[3]);
                    lstEmp.Add(objEmp);
                }
                if (lstEmp.Count > 0)
                {
                    return View(lstEmp);
                }
                
            }catch(Exception ex)
            {
                excepRepo.addException(ex);
                return View("Error");

            }
            return View();

        }


        public ActionResult AddEmployee()
        {
            EmployeeModel objEmp = new EmployeeModel();
            return View(objEmp);
        }
        public ActionResult SaveEmployee(EmployeeModel objEmp)
        {
            try {
                if (objEmp.Id > 0)
                {
                    XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Xml/EmployeeList.xml"));
                    var items = (from item in xmlDoc.Descendants("Employee") select item).ToList();
                    XElement selected = items.Where(p => p.Element("Id").Value == objEmp.Id.ToString()).FirstOrDefault();
                    selected.Remove();
                    xmlDoc.Save(Server.MapPath("~/Xml/EmployeeList.xml"));
                    xmlDoc.Element("Employees").Add(new XElement("Employee", new XElement("Id", objEmp.Id), new XElement("EmpName", objEmp.EmpName),
                        new XElement("Address", objEmp.Address), new XElement("Salary", objEmp.Salary)));
                    xmlDoc.Save(Server.MapPath("~/Xml/EmployeeList.xml"));
                    return RedirectToAction("listEmployees", "Employee");
                }
                else
                {
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.Load(Server.MapPath("~/XmL/EmployeeList.xml"));
                    XmlNodeList nodelist = oXmlDocument.GetElementsByTagName("Employee");
                    var x = oXmlDocument.GetElementsByTagName("Id");
                    int Max = 0;
                    foreach (XmlElement item in x)
                    {
                        int EId = Convert.ToInt32(item.InnerText.ToString());
                        if (EId > Max)
                        {
                            Max = EId;
                        }
                    }
                    Max = Max + 1;
                    XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XmL/EmployeeList.xml"));
                    xmlDoc.Element("Employees").Add(new XElement("Employee", new XElement("Id", Max), new XElement("EmpName", objEmp.EmpName),
                        new XElement("Address", objEmp.Address), new XElement("Salary", objEmp.Salary)));
                    xmlDoc.Save(Server.MapPath("~/XmL/EmployeeList.xml"));
                    return RedirectToAction("listEmployees", "Employee");
                }
            }catch(Exception ex)
            {
                excepRepo.addException(ex);
                return View("Error");
            }
            

            

        }

        public ActionResult GetDetailsById(int Id)
        {
            EmployeeModel objEmp = new EmployeeModel();
            XDocument oXmlDocument = XDocument.Load(Server.MapPath("~/Xml/EmployeeList.xml"));
            var items = (from item in oXmlDocument.Descendants("Employee")
                         where Convert.ToInt32(item.Element("Id").Value) == Id
                         select new EmployeeModel
                         {
                             Id = Convert.ToInt32(item.Element("Id").Value),
                            EmpName = item.Element("EmpName").Value,
                             Address = item.Element("Address").Value,
                             Salary =Convert.ToDecimal( item.Element("Salary").Value)
                         }).SingleOrDefault();
            if (items != null)
            {
                objEmp.Id = items.Id;
                objEmp.EmpName = items.EmpName;
                objEmp.Address = items.Address;
                objEmp.Salary = items.Salary;
            }
            return View("AddEmployee", objEmp);
        }

        public ActionResult DeleteEmployee(int Id)
        {
            if (Id > 0)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XmL/EmployeeList.xml"));
                var items = (from item in xmlDoc.Descendants("Employee") select item).ToList();
                XElement selected = items.Where(p => p.Element("Id").Value == Id.ToString()).FirstOrDefault();
                selected.Remove();
                xmlDoc.Save(Server.MapPath("~/XmL/EmployeeList.xml"));
            }
            return RedirectToAction("listEmployees", "Employee");
        }
    }
}