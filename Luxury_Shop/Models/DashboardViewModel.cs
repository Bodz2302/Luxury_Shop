using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Luxury_Shop.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalBrands { get; set; }  
        public int Totaldanggiao { get; set; }
        public int Totaldahuy { get; set; }       public int Totaldagiao { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Product> Products { get; set; } // Danh sách sản phẩm
        public List<Category> Categories { get; set; } // Danh sách danh mục
    }
}