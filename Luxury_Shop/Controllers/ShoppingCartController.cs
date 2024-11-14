﻿using System;
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
            cart.Add_Product_Cart(product, quantity);

            // Cập nhật giỏ hàng vào session
            Session["Cart"] = cart;

            // Quay lại trang hiển thị giỏ hàng
            return RedirectToAction("ShowCart");
        }
    }
}
