using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Luxury_Shop.Models
{
    public class AccountListViewModel
    {
        public IEnumerable<User> ListAcc { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<Product> Listproduct { get; set; }
        public IEnumerable<Order> listdonhang { get; set; }
        public IEnumerable<Category> listdanhmuc { get; set; }
        public IEnumerable<Brand> listhuonghiru { get; set; }
    }

}