using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Luxury_Shop.Models
{
    public enum PaymentMethodType
    {
        COD = 0,  // Cash On Delivery
        BankTransfer = 1  // Bank Transfer
    }

    public class OrderViewModel
    {
        public Cart Cart { get; set; } // Cart information

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; } // Full name of the customer

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; } // Phone number of the customer

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string Address { get; set; } // Delivery address

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public PaymentMethodType PaymentMethod { get; set; } // Payment method (COD or bank transfer)

        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải lớn hơn hoặc bằng 0.")]
        public decimal TotalAmount { get; set; } // Total amount for the order

        public List<CartItem> CartItems { get; set; } // List of items in the cart
    }
}
