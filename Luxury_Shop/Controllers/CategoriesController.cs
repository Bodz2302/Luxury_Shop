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
    public class CategoriesController : Controller
    {
        private LuxuryEntities1 db = new LuxuryEntities1();

        // GET: Categories
        [HttpGet]
        public ActionResult Index(int pageNumber = 1, int pageSize = 6)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            ViewBag.use = Session["username"];

            // Lấy tất cả danh mục cùng với các danh mục con
            var categories = db.Categories.ToList();



            var model = new AccountListViewModel
            {
                listdanhmuc = categories,
                TotalRecords = db.Categories.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(model);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            ViewBag.use = Session["username"];

            // Tạo danh sách các danh mục cha cho dropdown
            ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(c => c.ParentCategoryID == null), "CategoryID", "CategoryName");
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName,Description,ParentCategoryID")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(c => c.ParentCategoryID == null), "CategoryID", "CategoryName", category.ParentCategoryID);
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(c => c.ParentCategoryID == null), "CategoryID", "CategoryName", category.ParentCategoryID);
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName,Description,ParentCategoryID")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(c => c.ParentCategoryID == null), "CategoryID", "CategoryName", category.ParentCategoryID);
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
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
