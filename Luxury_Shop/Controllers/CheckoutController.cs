using Luxury_Shop.Controllers;
using Luxury_Shop.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

public class CheckoutController : Controller
{
    private readonly LuxuryEntities1 database = new LuxuryEntities1();
    

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
    public Task<ActionResult> ProcessPayment(OrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra thông tin.";
            return Task.FromResult<ActionResult>(RedirectToAction("Index"));
        }

        Cart cart = Session["Cart"] as Cart;
        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
            return Task.FromResult<ActionResult>(RedirectToAction("Index"));
        }

        try
        {
            string orderId = GenerateOrderId();
            SaveOrderToDatabase(orderId, "confirmed", model);

            if (model.PaymentMethod == PaymentMethodType.BankTransfer)
            {
                TempData["OrderID"] = orderId;
                TempData["TotalAmount"] = cart.GetTotalAmount();
                return Task.FromResult<ActionResult>(RedirectToAction("BankTransfer", "Checkout"));
            }

            Session["Cart"] = null;
            return Task.FromResult<ActionResult>(RedirectToAction("SuccessCheckout"));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Đã xảy ra lỗi khi xử lý đơn hàng: " + ex.Message;
            return Task.FromResult<ActionResult>(RedirectToAction("Index"));
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
    public string GenerateOrderId()
    {
        // Kiểm tra xem mã đơn hàng đã tồn tại trong session chưa
        if (HttpContext.Session["OrderId"] == null)
        {
            // Tạo một số ngẫu nhiên gồm 4 chữ số
            Random random = new Random();
            int randomPart = random.Next(1000, 10000); // Sinh ra một số ngẫu nhiên từ 1000 đến 9999

            // Kết hợp 4 chữ số đầu là "2024" với phần ngẫu nhiên
            string orderId = "2024" + randomPart.ToString();

            // Lưu mã đơn hàng vào session
            HttpContext.Session["OrderId"] = orderId;
        }

        // Lấy mã đơn hàng từ session
        return HttpContext.Session["OrderId"].ToString();
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

    // Phương thức gửi tin nhắn Telegram
    

    // Trang success checkout
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
        // Lấy giỏ hàng từ Session
        var cart = Session["Cart"] as Cart;
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("Index", "ShoppingCart"); // Nếu giỏ hàng trống, quay về giỏ hàng
        }

        // Tính tổng tiền và mã đơn hàng
        decimal totalAmount = cart.Items.Sum(item => item.Quantity * item.Product.OriginalPrice);
        string orderCode = DateTime.Now.ToString("yyyyMMddHHmmss");

        // Lưu thông tin đơn hàng vào TempData để sử dụng khi kiểm tra thanh toán
        TempData["TotalAmount"] = totalAmount;
        TempData["OrderCode"] = orderCode;

        // Truyền dữ liệu qua View để khách hàng chuyển khoản
        ViewBag.TotalAmount = totalAmount;
        ViewBag.OrderCode = orderCode;

        return View();
    }

    public async Task<ActionResult> CheckPaymentStatus()
    {
        string orderCode = TempData["OrderCode"]?.ToString();
        decimal totalAmount = Convert.ToDecimal(TempData["TotalAmount"] ?? 0);

        // Giả sử bạn có một phương thức kiểm tra thanh toán qua API
        var paymentStatus = await CheckPaymentApi(orderCode);

        if (paymentStatus == "Success")
        {
            // Cập nhật đơn hàng thành công và chuyển sang trang thành công
            TempData["SuccessMessage"] = "Thanh toán thành công! Cảm ơn bạn đã mua hàng!";
            return RedirectToAction("SuccessCheckout");
        }
        else
        {
            // Nếu thanh toán không thành công, hiển thị lỗi
            TempData["ErrorMessage"] = "Thanh toán không thành công. Vui lòng thử lại!";
            return RedirectToAction("FailedPayment");
        }
    }

    private async Task<string> CheckPaymentApi(string orderCode)
    {
        // Giả sử bạn gọi một API để kiểm tra trạng thái thanh toán của đơn hàng
        await Task.Delay(2000); // Giả lập delay
        return "Success"; // Giả sử trả về trạng thái thành công
    }
}
