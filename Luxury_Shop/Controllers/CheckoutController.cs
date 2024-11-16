using Luxury_Shop.Models;
using System;
using System.Linq;
using System.Web.Mvc;

public class CheckoutController : Controller
{
    private readonly LuxuryEntities1 database = new LuxuryEntities1();

    // GET: Checkout/Index
    // GET: Checkout/Index
    public ActionResult Index()
    {
        // Kiểm tra xem Cart đã tồn tại trong session hay chưa
        Cart cart = Session["Cart"] as Cart;
        if (cart == null)
        {
            // Nếu giỏ hàng chưa được khởi tạo, tạo một giỏ hàng mới và thêm vào session
            cart = new Cart();
            Session["Cart"] = cart;
        }

        if (!cart.Items.Any())
        {
            // Nếu giỏ hàng rỗng, chuyển hướng về trang giỏ hàng
            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        // Tạo CheckoutViewModel để truyền vào view
        var viewModel = new CheckoutViewModel
        {
            CartItems = cart.Items,
            TotalAmount = GetTotalAmount()
        };

        return View(viewModel);
    }


    // POST: Checkout/ProcessPayment
    [HttpPost]
    public ActionResult ProcessPayment(CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { status = "failed", message = "Vui lòng điền đầy đủ thông tin." });
        }

        // Lấy giỏ hàng từ session
        Cart cart = Session["Cart"] as Cart;
        if (cart == null || !cart.Items.Any())
        {
            return Json(new { status = "failed", message = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán." });
        }

        // Tạo mã đơn hàng ngẫu nhiên
        string orderId = GenerateOrderId();

        // Kiểm tra phương thức thanh toán
        if (model.PaymentMethod == 1) // COD - Thanh toán khi nhận hàng
        {
            // Lưu đơn hàng vào cơ sở dữ liệu với trạng thái "confirmed"
            SaveOrderToDatabase(orderId, "confirmed", model);

            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["Cart"] = null;

            return Json(new { status = "success", message = "Đơn hàng đã được xác nhận. Vui lòng chuẩn bị thanh toán khi nhận hàng." });
        }

        return Json(new { status = "failed", message = "Phương thức thanh toán không hợp lệ." });
    }

    // Phương thức lưu đơn hàng vào cơ sở dữ liệu
    private void SaveOrderToDatabase(string orderId, string status, CheckoutViewModel model)
    {
        try
        {
            // Chuyển đổi OrderID sang kiểu int
            if (!int.TryParse(orderId, out int numericOrderId))
            {
                throw new Exception("Order ID không hợp lệ. Không thể chuyển đổi sang số nguyên.");
            }

            // Tạo đối tượng Order
            Order order = new Order
            {
                OrderID = numericOrderId,
                Status = status,
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };

            // Lưu vào cơ sở dữ liệu
            database.Orders.Add(order);
            database.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("Lỗi khi lưu đơn hàng: " + ex.Message);
        }
    }

    // Phương thức tạo mã đơn hàng ngẫu nhiên
    private string GenerateOrderId()
    {
        // Tạo một số ngẫu nhiên gồm 4 chữ số
        Random random = new Random();
        int randomPart = random.Next(1000, 10000); // Sinh ra một số ngẫu nhiên từ 1000 đến 9999

        // Kết hợp 4 chữ số đầu là "2024" với phần ngẫu nhiên
        string orderId = "2024" + randomPart.ToString();

        return orderId;
    }

    // Phương thức tính tổng số tiền trong giỏ hàng
    private decimal GetTotalAmount()
    {
        // Lấy giỏ hàng từ session
        Cart cart = Session["Cart"] as Cart;
        if (cart == null || cart.Items == null || !cart.Items.Any())
        {
            return 0;
        }

        // Tính tổng số tiền của giỏ hàng
        return cart.Items.Sum(item => item.Product.OriginalPrice * item.Quantity);
    }
}
