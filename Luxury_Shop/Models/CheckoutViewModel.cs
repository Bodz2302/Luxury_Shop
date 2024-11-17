using System.Collections.Generic;
using Luxury_Shop.Models; // Add this using directive

public class CheckoutViewModel
{
    public List<CartItem> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
    public string FullName { get; set; } // Họ tên
    public string PhoneNumber { get; set; } // Số điện thoại
    public string Address { get; set; } // Địa chỉ nhà
    public int PaymentMethod { get; set; } // Phương thức thanh toán
}
