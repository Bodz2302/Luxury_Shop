using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Luxury_Shop.Models;

namespace Luxury_Shop.Controllers
{
    public class HomePageController : Controller
    {
        private LuxuryEntities1 database = new LuxuryEntities1();
        // GET: HomePage
        public ActionResult HomePage()
        {
            // Lấy tất cả sản phẩm từ cơ sở dữ liệu
            var products = database.Products.ToList();

            // Trả dữ liệu cho view
            return View(products);
        }
        public ActionResult Gucci()
        {
            return View();
        }
    }
}