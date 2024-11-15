using System;
using System.Linq;
using System.Web.Mvc;
using Luxury_Shop.Models;

namespace Luxury_Shop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private LuxuryEntities1 database = new LuxuryEntities1();

        // GET: ShoppingCart/ShowCart
        public ActionResult ShowCart()
        {
            ViewBag.check = Session["check"];
            if (ViewBag.check == null)
            {
                ViewBag.mes = "Vui lòng đăng nhập để mua hàng và xem giỏ hàng";
                return View();
            }

            if (Session["Cart"] == null)
            {
                Session["Cart"] = new Cart();
            }

            Cart _cart = Session["Cart"] as Cart;
            return View(_cart);
        }

        // POST: ShoppingCart/AddToCart
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = database.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                return HttpNotFound("Product not found");
            }

            var cart = Session["Cart"] as Cart ?? new Cart();
            cart.AddProductToCart(product, quantity);
            Session["Cart"] = cart;
            return RedirectToAction("ShowCart");
        }

        // GET: ShoppingCart/Checkout
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("ShowCart");
            }
            return View(cart);
        }

        // POST: ShoppingCart/ProcessCheckout
        [HttpPost]
        public ActionResult ProcessCheckout()
        {
            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("ShowCart");
            }

            // Xử lý logic thanh toán tại đây
            Session["Cart"] = null;
            return RedirectToAction("OrderConfirmation");
        }

        // GET: ShoppingCart/OrderConfirmation
        public ActionResult OrderConfirmation()
        {
            return View();
        }

        // PHƯƠNG THỨC LẤY TỔNG SỐ TIỀN TRONG GIỎ HÀNG
        public decimal GetTotalAmount()
        {
            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any())
            {
                return 0;
            }

            // Tính tổng số tiền từ các sản phẩm trong giỏ
            return cart.Items.Sum(item => item.Product.OriginalPrice * item.Quantity);
        }
    }
}
