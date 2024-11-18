using Luxury_Shop.Models;
using System;
using System.Linq;
using System.Web.Mvc;

public class CheckoutController : Controller
{
    private readonly LuxuryEntities1 database = new LuxuryEntities1();

    // GET: Checkout/Index
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

        // Tạo OrderViewModel để truyền vào view
        var orderViewModel = new OrderViewModel
        {
            CartItems = cart.Items,
            TotalAmount = GetTotalAmount() // Giả sử phương thức này tính tổng tiền trong giỏ hàng
        };

        return View(orderViewModel);
    }





    // POST: Checkout/ProcessPayment
    [HttpPost]
    public ActionResult ProcessPayment(OrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Trả về lại view với thông báo lỗi
            TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra thông tin.";
            return RedirectToAction("Index");
        }

        // Lấy giỏ hàng từ session
        Cart cart = Session["Cart"] as Cart;
        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
            return RedirectToAction("Index");
        }

        try
        {
            // Tạo mã đơn hàng
            string orderId = GenerateOrderId();

            // Lưu đơn hàng vào cơ sở dữ liệu, bao gồm PaymentMethod
            SaveOrderToDatabase(orderId, "confirmed", model);

            // Kiểm tra phương thức thanh toán
            if (model.PaymentMethod == PaymentMethodType.BankTransfer) // 1 = Chuyển khoản ngân hàng
                                                                       // 1 = Chuyển khoản ngân hàng
            {
                // Lưu thông tin cần thiết vào TempData để hiển thị trên trang BankTransfer
                TempData["OrderID"] = orderId;
                TempData["TotalAmount"] = cart.GetTotalAmount();

                // Chuyển hướng đến trang BankTransfer
                return RedirectToAction("BankTransfer", "Checkout");
            }

            // Nếu thanh toán COD, tiếp tục xử lý
            // Xóa giỏ hàng sau khi đặt hàng thành công
            Session["Cart"] = null;

            // Chuyển hướng tới trang SuccessCheckout
            return RedirectToAction("SuccessCheckout");
        }
        catch (Exception ex)
        {
            // Ghi lỗi vào log nếu cần
            TempData["Error"] = "Đã xảy ra lỗi khi xử lý đơn hàng: " + ex.Message;
            return RedirectToAction("Index");
        }
    }



    // Phương thức lưu đơn hàng vào cơ sở dữ liệu
    private void SaveOrderToDatabase(string orderId, string status, OrderViewModel model)
    {
        try
        {
            // Chuyển đổi OrderID sang kiểu int (nếu cần thiết)
            if (!int.TryParse(orderId, out int numericOrderId))
            {
                throw new Exception("Order ID không hợp lệ. Không thể chuyển đổi sang số nguyên.");
            }

            // Kiểm tra tính hợp lệ của số điện thoại
            if (string.IsNullOrEmpty(model.PhoneNumber) || !model.PhoneNumber.All(char.IsDigit))
            {
                throw new Exception("Phone number không hợp lệ. Vui lòng nhập đúng định dạng số.");
            }

            // Chuyển đổi số điện thoại từ chuỗi sang số nguyên
            int phoneNumber = int.Parse(model.PhoneNumber);

            // Tạo đối tượng Order từ OrderViewModel
            Order order = new Order
            {
                OrderID = numericOrderId,
                Status = status,
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                FullName = model.FullName, // Sử dụng thuộc tính FullName từ OrderViewModel
                PhoneNumber = phoneNumber, // Chuyển đổi số điện thoại sang số nguyên
                ShippingAddress = model.Address, // Sử dụng thuộc tính Address từ OrderViewModel
                PaymentMethod = model.PaymentMethod.ToString() // Chuyển đổi PaymentMethod sang chuỗi
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
    public ActionResult SuccessCheckout()
    {
        return View();
    }
    public ActionResult FailedPayment()
    {
        return View();
    }

    public ActionResult BankTransfer()
    {
        // Giả sử bạn có lớp `Cart` chứa các mặt hàng trong giỏ hàng
        var cart = Session["Cart"] as Cart; // Lấy giỏ hàng từ Session
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("Index", "Cart"); // Chuyển về giỏ hàng nếu trống
        }

        // Tính tổng tiền
        decimal totalAmount = cart.Items.Sum(item => item.Quantity * item.Product.OriginalPrice);

        // Tạo mã đơn hàng (giả sử mã đơn hàng là ngày giờ hiện tại)
        string orderCode = DateTime.Now.ToString("yyyyMMddHHmmss");

        // Truyền dữ liệu sang View
        ViewBag.TotalAmount = totalAmount;
        ViewBag.OrderCode = orderCode;

        return View();
    }

}
