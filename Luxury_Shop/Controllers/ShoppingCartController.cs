using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Luxury_Shop.Models;



namespace Luxury_Shop.Controllers
{
   
    public class ShoppingCartController : Controller
    {
        private LuxuryEntities database = new LuxuryEntities();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowCart()
        {
            if (Session["Cart"]==null)
                return RedirectToAction("ShowCart", "ShoppingCart");
            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);
        }
    }
}