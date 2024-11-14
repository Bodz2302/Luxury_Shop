using System.Collections.Generic;
using System.Linq;

namespace Luxury_Shop.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddProductToCart(Product product, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.Product.ProductID == product.ProductID);
            if (item != null)
            {
                // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng
                item.Quantity += quantity;
            }
            else
            {
                // Nếu sản phẩm chưa có, thêm mới vào giỏ hàng
                Items.Add(new CartItem { Product = product, Quantity = quantity });
            }
        }

        // Phương thức để tính tổng số tiền của giỏ hàng
        public decimal Total_Money()
        {
            return Items.Sum(item => item.Product.OriginalPrice * item.Quantity);
        }
    }

    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
