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
    }
}
