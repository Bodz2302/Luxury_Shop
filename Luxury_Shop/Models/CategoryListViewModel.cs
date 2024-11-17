using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Luxury_Shop.Models
{
    public class CategoryListViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
