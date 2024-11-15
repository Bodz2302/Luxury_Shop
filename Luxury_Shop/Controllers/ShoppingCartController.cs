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
            // Kiểm tra nếu giỏ hàng không tồn tại trong session
            if (Session["Cart"] == null)
            {
                // Nếu giỏ hàng không có, tạo mới và lưu vào session
                Session["Cart"] = new Cart();
            }

            // Lấy giỏ hàng từ session
            Cart _cart = Session["Cart"] as Cart;

            // Trả về view và truyền giỏ hàng vào view
            return View(_cart);
        }

        // POST: ShoppingCart/AddToCart
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            // Lấy sản phẩm từ database
            var product = database.Products.FirstOrDefault(p => p.ProductID == productId);

            // Kiểm tra sản phẩm có tồn tại hay không
            if (product == null)
            {
                return HttpNotFound("Product not found");
            }

            // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
            var cart = Session["Cart"] as Cart ?? new Cart();

            // Thêm sản phẩm vào giỏ hàng
            cart.AddProductToCart(product, quantity);

            // Cập nhật giỏ hàng vào session
            Session["Cart"] = cart;

            // Quay lại trang hiển thị giỏ hàng
            return RedirectToAction("ShowCart");
        }

        // GET: ShoppingCart/Checkout
        public ActionResult Checkout()
        {
            // Lấy giỏ hàng từ session
            var cart = Session["Cart"] as Cart;

            // Kiểm tra nếu giỏ hàng trống hoặc không có sản phẩm nào
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("ShowCart");
            }

            // Trả về view Checkout
            return View(cart);
        }

        // POST: ShoppingCart/ProcessCheckout
        [HttpPost]
        public ActionResult ProcessCheckout()
        {
            // Lấy giỏ hàng từ session
            var cart = Session["Cart"] as Cart;

            // Kiểm tra nếu giỏ hàng trống
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("ShowCart");
            }

            // Xử lý logic thanh toán tại đây
            // Ví dụ: lưu thông tin đơn hàng vào database

            // Xóa giỏ hàng sau khi thanh toán thành công
            Session["Cart"] = null;

            // Điều hướng đến trang cảm ơn hoặc xác nhận thanh toán
            return RedirectToAction("OrderConfirmation");
        }

        // GET: ShoppingCart/OrderConfirmation
        public ActionResult OrderConfirmation()
        {
            // Trả về trang xác nhận đơn hàng
            return View();
        }
    }
}
