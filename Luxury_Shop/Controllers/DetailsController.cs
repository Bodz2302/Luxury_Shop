using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Luxury_Shop.Models;

namespace Luxury_Shop.Controllers
{
    public class DetailsController : Controller
    {
        // GET: Details
        private LuxuryEntities1 database = new LuxuryEntities1();
        [HttpGet]
        public ActionResult Details(int id)
        {
            var product = database.Products
                                   .Include(p => p.Category)  // Load thông tin category
                                   .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();  // Nếu sản phẩm không tồn tại
            }

            return View(product);
        }
    }
}