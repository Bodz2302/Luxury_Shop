using System;
using System.Collections.Generic;
using System.Linq;
using Luxury_Shop.Models;
namespace Luxury_Shop.Models
{

    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    public class Cart
    {
        private LuxuryEntities1 db = new LuxuryEntities1();
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }

        public void Add_Product_Cart(Product pro, int quan = 1)
        {

            var item = items.FirstOrDefault(p => p.Product.ProductID == pro.ProductID);
            if (item == null)
            {
                items.Add(new CartItem
                {
                    Product = pro,
                    Quantity = quan
                });
            }
            else
            {
                item.Quantity += quan;
            }
        }

        public void Remove_Product_Cart(Product pro)
        {
            items.RemoveAll(p => p.Product.ProductID == pro.ProductID);
        }

        public int Total_quantity()
        {
            return items.Sum(p => p.Quantity);
        }

        public decimal Total_Money()
        {
            return items.Sum(p => p.Quantity * p.Product.OriginalPrice);
        }

        public void Update_quantity(int id, int quan)
        {
            var item = items.Find(p => p.Product.ProductID == id);
            if (item != null)
                item.Quantity = quan;
        }

        public void Clear_cart()
        {
            items.Clear();
        }
    }
}
