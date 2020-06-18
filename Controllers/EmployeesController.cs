using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Employee_Manager_App.Models;
using Microsoft.Build.Framework;

namespace Employee_Manager_App.Controllers
{
    public class EmployeesController : Controller
    {
        private Employee_Manager_DBEntities1 db = new Employee_Manager_DBEntities1();
        
        // GET: Employees
        public ActionResult Index()
        {
            if (HttpContext.Session["error"] != null)
            {
                ViewBag.Error = HttpContext.Session["error"];
                HttpContext.Session["error"] = null;
            }
           
            var employees = db.Employees.Include(e => e.Department1).Include(e => e.Manager1).Include(e => e.Position1);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.Department = new SelectList(db.Departments, "ID", "Department_Name");
            var managers = db.Managers.Include(e => e.Employee).ToList();
            managers.Add(null);
            ViewBag.Manager = new SelectList(managers, "ID", "Employee.Name");
            ViewBag.Position = new SelectList(db.Positions, "ID", "Position1");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Address,Email,Phone,Position,Department,Start_Date,Shift,Manager,Photo,Fav_Color")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.Status = true;
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Department = new SelectList(db.Departments, "ID", "Department_Name", employee.Department);
            ViewBag.Manager = new SelectList(db.Managers, "ID", "ID", employee.Manager);
            ViewBag.Position = new SelectList(db.Positions, "ID", "Position1", employee.Position);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department = new SelectList(db.Departments, "ID", "Department_Name", employee.Department);
            var managers = db.Managers.Include(e => e.Employee).ToList();
            managers.Add(null);
            ViewBag.Manager = new SelectList(managers, "ID", "Employee.Name");
            ViewBag.Position = new SelectList(db.Positions, "ID", "Position1", employee.Position);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Address,Email,Phone,Position,Department,Start_Date,End_Date,Status,Shift,Manager,Photo,Fav_Color")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                //Set up for adding entries to the log file.
                var logfile = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "Log.txt";
                var writer = new System.IO.StreamWriter(logfile, true);

                var oldemp = db.Employees.Find(employee.ID);
                if (oldemp.Status == true && employee.Status == false)
                {
                    employee.End_Date = DateTime.Today;
                    writer.WriteLine(DateTime.Now.ToString() + " " + employee.Name + " terminated.");

                }
                if (oldemp.Manager != employee.Manager)
                {
                    writer.WriteLine(DateTime.Now.ToString() + " " + employee.Name + " New Manager: " + employee.Manager1.Employee.Name.ToString());
                }
                if (oldemp.Position != employee.Position)
                {
                    writer.WriteLine(DateTime.Now.ToString() + " " + employee.Name + " New Position: " + employee.Position.ToString());
                }
                writer.Close();
                employee.Start_Date = oldemp.Start_Date;
                db.Entry(oldemp).State = EntityState.Detached;
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Department = new SelectList(db.Departments, "ID", "Department_Name", employee.Department);
            ViewBag.Manager = new SelectList(db.Managers, "ID", "ID", employee.Manager);
            ViewBag.Position = new SelectList(db.Positions, "ID", "Position1", employee.Position);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Employee employee = db.Employees.Find(id);
                db.Employees.Remove(employee);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                HttpContext.Session["error"] = "Unable to delete. Dependency exists in DB";
            }
            return RedirectToAction("Index");
        }

        //GET: Employees/Retention
        public ActionResult Retention()
        {
            var employees = db.Employees.Include(e => e.Department1).Include(e => e.Manager1).Include(e => e.Position1);
            var emp_list = employees.ToList();
            //var dates = new List<Tuple<DateTime, DateTime?, Boolean>>();
            //foreach (var entry in emp_list){
            //    dates.Add(new Tuple<DateTime, DateTime?, bool>(entry.Start_Date, entry.End_Date, entry.Status));
            //}
            //TempData["employees"] = dates;
            return View(emp_list);
        }

        public ActionResult Log()
        {
            string[] lines = System.IO.File.ReadAllLines(Server.MapPath("~/App_Data/Log.txt"));
            ViewBag.Lines = lines;
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
