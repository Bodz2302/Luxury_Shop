using Luxury_Shop.Controllers;
using Luxury_Shop.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        if (string.IsNullOrEmpty(orderCode) || totalAmount <= 0)
        {
            TempData["Error"] = "Thông tin đơn hàng không hợp lệ.";
            return RedirectToAction("FailedPayment");
        }

        // Gọi API để lấy lịch sử giao dịch
        var pay2sController = new Pay2sController();
        string responseJson = await pay2sController.FetchTransactions(
            DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"),
            DateTime.Now.ToString("dd/MM/yyyy")
        );

        // Parse JSON từ Pay2s API
        dynamic transactions = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson);

        if (transactions != null && transactions.status == true)
        {
            foreach (var transaction in transactions.data)
            {
                // Kiểm tra giao dịch có trùng OrderCode và tổng tiền không
                if (transaction.description.Contains(orderCode) &&
                    Convert.ToDecimal(transaction.amount) == totalAmount)
                {
                    // Đánh dấu đơn hàng đã thanh toán thành công
                    TempData["Success"] = "Thanh toán thành công!";
                    return RedirectToAction("SuccessCheckout");
                }
            }
        }

        TempData["Error"] = "Không tìm thấy giao dịch phù hợp. Vui lòng kiểm tra lại.";
        return RedirectToAction("FailedPayment");
    }



}
