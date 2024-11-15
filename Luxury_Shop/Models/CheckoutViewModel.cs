using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Luxury_Shop.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
    }

}