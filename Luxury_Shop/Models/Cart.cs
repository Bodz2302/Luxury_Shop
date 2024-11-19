using System;
using System.Collections.Generic;
using System.Linq;

namespace Luxury_Shop.Models
{
    public class Cart
    {
        // Danh sách các sản phẩm trong giỏ hàng
        public List<CartItem> Items { get; set; }

        // Constructor khởi tạo giỏ hàng
        public Cart()
        {
            Items = new List<CartItem>();
        }

        // Phương thức thêm sản phẩm vào giỏ
        public void AddToCart(Product product, int quantity)
        {
            // Kiểm tra nếu sản phẩm đã có trong giỏ
            var existingItem = Items.FirstOrDefault(item => item.Product.ProductID == product.ProductID);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Nếu sản phẩm đã tồn tại, tăng số lượng
            }
            else
            {
                // Nếu chưa có sản phẩm trong giỏ, thêm sản phẩm mới vào
                Items.Add(new CartItem { Product = product, Quantity = quantity });
            }
        }

        // Phương thức xóa sản phẩm khỏi giỏ hàng
        public void RemoveFromCart(int productId)
        {
            var item = Items.FirstOrDefault(i => i.Product.ProductID == productId);
            if (item != null)
            {
                Items.Remove(item); // Xóa sản phẩm khỏi giỏ
            }
        }

        // Phương thức tính tổng số tiền trong giỏ hàng
        public decimal GetTotalAmount()
        {
            // Tính tổng tiền của giỏ hàng
            return Items.Sum(item => item.Product.OriginalPrice * item.Quantity);
        }

        // Phương thức kiểm tra nếu giỏ hàng có sản phẩm hay không
        public bool HasItems()
        {
            return Items.Any(); // Kiểm tra giỏ hàng có sản phẩm không
        }
    }

    public class CartItem
    {
        // Sản phẩm trong giỏ
        public Product Product { get; set; }
        // Số lượng sản phẩm trong giỏ
        public int Quantity { get; set; }
        public string Size { get; set; }
    }
}
