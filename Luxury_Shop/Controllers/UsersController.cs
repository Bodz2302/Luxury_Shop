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
            if (Session["username"] == null)
            {
                return RedirectToAction("Index", "Users");
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
            ViewBag.Admin = Session["admin"];
            ViewBag.check = Session["check"];  // Sửa thành check thay vì UserID
            ViewBag.use = Session["username"];
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
            if (Session["check"] != null && Session["check"].ToString() == "True")
            {
                return RedirectToAction("Index", "Users");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu";
                return View(new User { Username = username }); // Trả lại dữ liệu đã nhập
            }
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View(new User { Username = username });
            }
            Session["username"] = user.Username;
            Session["check"] = true;
            ViewBag.use = Session["username"];
            if (user.IsAdmin.HasValue && user.IsAdmin.Value)
            { 
                Session["admin"] = true;
                return RedirectToAction("Dashboard", "Users");
            }
            else
            {
                return RedirectToAction("HomePage", "HomePage");
            }
        }
        public ActionResult Create()
        {          
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Username,Password,Email,FullName,Phone,Address,CreatedAt,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được đăng ký");
                }
                if (db.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã được đăng ký");
                }
                if (db.Users.Any(u => u.Phone == user.Phone))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại này đã được đăng ký");
                }
                if (ModelState.IsValid)
                {
                    user.IsAdmin = false;
                    user.CreatedAt = DateTime.Now;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View(user);
        }
        public ActionResult taomoi()
        {         
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
            ViewBag.check = Session["check"];  // Sửa thành check thay vì UserID
            ViewBag.use = Session["username"];
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Username,Password,Email,FullName,Phone,Address,CreatedAt,IsAdmin")] User user)
        {
            ViewBag.Admin = Session["admin"];
            ViewBag.check = Session["check"];  // Sửa thành check
            ViewBag.use = Session["username"];
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = db.Users.Find(user.UserID);
                    if (existingUser == null)
                    {
                        return HttpNotFound();
                    }
                    if (Session["admin"] == null || (bool)Session["admin"] == false)
                    {
                        user.IsAdmin = existingUser.IsAdmin;
                    }
                    if (!string.IsNullOrEmpty(user.Password) && user.Password != existingUser.Password)
                    {
                        existingUser.Password = user.Password;
                    }
                    existingUser.Email = user.Email;
                    existingUser.FullName = user.FullName;
                    existingUser.Phone = user.Phone;
                    existingUser.Address = user.Address;
                    existingUser.IsAdmin = user.IsAdmin;
                    db.SaveChanges();
                    if (Session["admin"] == null)
                    {
                        return RedirectToAction("Index", "Users");
                    }
                    return RedirectToAction("Quanlykh");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                    System.Diagnostics.Debug.WriteLine("Error in Edit: " + ex.ToString());
                    return View(user);
                }
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
