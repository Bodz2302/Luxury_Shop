using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Luxury_Shop.Models;

namespace Luxury_Shop.Controllers
{
    public class UsersController : Controller
    {
        private LuxuryEntities1 db = new LuxuryEntities1();

        // GET: Users
        public ActionResult Dashboard()
        {
            if (Session["admin"] == null)
            {

                {
                    return RedirectToAction("HomePage", "HomePage");
                }
            }
            ViewBag.use = Session["username"];
            ViewBag.check = Session["check"];
            ViewBag.Admin = Session["admin"];
            var soucess=     db.Orders.Where(c => c.Status == "Completed").ToList();
            var custumer = db.Users.Where(c => c.IsAdmin == false).ToList();
            var chogiao = db.Orders.Where(c => c.Status == "Pending").ToList();
            var fall=     db.Orders.Where(c => c.Status == "Đã Hủy").ToList();
            var dashboardData = new DashboardViewModel
            { Totaldagiao = soucess.Count(),
            Totaldahuy=fall.Count(),    Totaldanggiao = chogiao.Count(),
                TotalBrands = db.Brands.Count(),
                TotalProducts = db.Products.Count(),
                TotalCategories = db.Categories.Count(),
                TotalCustomers = custumer.Count(),
                TotalOrders = db.Orders.Count(),
                TotalRevenue = (decimal)(db.Orders.Sum(o => o.TotalAmount) ?? 0),
                Products = db.Products.ToList(), 
                Categories = db.Categories.ToList() 
            };

          
            return View(dashboardData);
          
        }
        public ActionResult Index()
        {
            ViewBag.use = Session["username"];
            ViewBag.check = Session["check"];
            ViewBag.Admin = Session["admin"];
            if (Session["admin"] == null)
            {

                {
                    return RedirectToAction("loi", "Users");
                }
            }
            string use = Session["username"].ToString();
            var user = db.Users.SingleOrDefault(u => u.Username == use);
            ViewBag.tk = user;
            return View(user);
        }
        [HttpGet]
        public ActionResult Quanlykh(int pageNumber = 1, int pageSize = 6)
        {
            ViewBag.use = Session["username"];
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            ViewBag.use = Session["username"];
            var totalRecords = db.Users.Count();
            var accounts = db.Users
                                   .OrderBy(a => a.UserID)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

            var model = new AccountListViewModel
            {
                ListAcc = accounts,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(model);
        }
        public ActionResult loi()
        {


            return View();
        }
        public ActionResult Quanlykh()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }

            return View(db.Users.ToList());
        }
        public ActionResult logout()
        {
            Session["username"] = null;
            Session["admin"] = false; ;
            Session["check"] = false;
            Session.Clear();
            return RedirectToAction("HomePage", "HomePage");

        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["check"] != null)
            {
                if (Session["check"].ToString() == "True")
                {
                    return RedirectToAction("Index", "Users");
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                {
                    {
                        var data = db.Users.FirstOrDefault(u => u.Username == email && u.Password == password);
                        if (data != null)
                        {
                            try { 
                            if (data.IsAdmin.Value == null)
                            {
                                data.IsAdmin = false;
                            }
                            }
                            catch { data.IsAdmin = false; }
                            Session["username"] = email;
                            Session["check"] = true; ViewBag.use = Session["username"];
                            if (data.IsAdmin.Value == true)
                            {
                                Session["admin"] = true;
                            }
                            return RedirectToAction("Dashboard", "Users");
                        }
                        else
                        {
                            Session["check"] = false;
                            Session["ErrorMessage"] = "Thông tin đăng nhập không hợp lệ.";
                            return RedirectToAction("Login");
                        }
                    }
                }
            }
            return View();
        }
        // GET: Users/Create
        public ActionResult Create()
        {
            if (Session["admin"] == null)
            {

                {
                    return RedirectToAction("loi", "Users");
                }
            }
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Username,Password,Email,FullName,Phone,Address,CreatedAt,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                user.IsAdmin = false;
                user.CreatedAt = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }
        public ActionResult taomoi()
        {
            if (Session["admin"] == null)
            {

                {
                    return RedirectToAction("loi", "Users");
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult taomoi([Bind(Include = "UserID,Username,Password,Email,FullName,Phone,Address,CreatedAt,IsAdmin")] User user)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            if (ModelState.IsValid)
            {

                user.CreatedAt = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Quanlykh");
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(user);
        }
        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Admin = Session["admin"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Username,Password,Email,FullName,Phone,Address,CreatedAt,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                if (Session["admin"] == null)
                {
                    return RedirectToAction("Index", "Users");
                }
                return RedirectToAction("Quanlykh");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            user.IsAdmin = false;
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Quanlykh");
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
