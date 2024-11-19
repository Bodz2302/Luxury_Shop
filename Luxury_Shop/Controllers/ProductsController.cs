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
    public class ProductsController : Controller
    {
        private LuxuryEntities1 db = new LuxuryEntities1();

        public ActionResult ViewProductsByCategory(int categoryId)
        {
            var productsInCategory = db.Products.Where(p => p.CategoryID == categoryId).ToList();
            return View(productsInCategory); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult ViewProductsByBrands(int brandId)
        {
            var productsInBrands = db.Products.Where(p => p.BrandID == brandId).ToList();
            return View(productsInBrands); // Truyền danh sách sản phẩm vào view
        }
        public ActionResult ViewProductsByCategoryAndBrand(int categoryId, int brandId)
        {
            // Lọc sản phẩm theo cả CategoryID và BrandID
            var filteredProducts = db.Products
                .Where(p => p.CategoryID == categoryId && p.BrandID == brandId)
                .ToList();

            // Trả về view với danh sách sản phẩm được lọc
            return View(filteredProducts);
        }
        // GET: Products1
        [HttpGet]
        public ActionResult Index(int pageNumber = 1, int pageSize = 6)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            ViewBag.use = Session["username"];
            var totalRecords = db.Products.Count();
            var accounts = db.Products
                                   .OrderBy(a => a.ProductID)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

            var model = new AccountListViewModel
            {
                Listproduct = accounts,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(model);
        }

        // GET: Products1/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            ViewBag.use = Session["username"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products1/Create
        public ActionResult Create()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            ViewBag.use = Session["username"];
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,Description,CategoryID,BrandID,OriginalPrice,SalePrice,DiscountPercentage,StockQuantity,ImageURL,CreatedAt,UpdatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("loi", "Users");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,Description,CategoryID,BrandID,OriginalPrice,SalePrice,DiscountPercentage,StockQuantity,ImageURL,CreatedAt,UpdatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.UpdatedAt = DateTime.Now;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
