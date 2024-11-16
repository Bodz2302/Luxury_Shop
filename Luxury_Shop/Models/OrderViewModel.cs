using System.Collections.Generic;

namespace Luxury_Shop.Models
{
    public class OrderViewModel
    {
        // Add Cart property if you want to pass the Cart object to the view
        public Cart Cart { get; set; } // Cart information
        public string FullName { get; set; } // Full name of the customer
        public string PhoneNumber { get; set; } // Phone number of the customer
        public string Address { get; set; } // Delivery address
        public int PaymentMethod { get; set; } // Payment method (COD or bank transfer)
        public decimal TotalAmount { get; set; } // Total amount for the order
        public List<CartItem> CartItems { get; set; } // List of items in the cart
    }
}
