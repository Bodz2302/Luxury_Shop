using System;
using System.Threading.Tasks;
using System.Web.Mvc;

public class CheckoutController : Controller
{
    private readonly Pay2SService _pay2SService;

    public CheckoutController()
    {
        _pay2SService = new Pay2SService(); // Khởi tạo đối tượng Pay2SService
    }

    // GET: Checkout/Index
    public ActionResult Index()
    {
        return View();
    }

    // POST: Checkout/ProcessPayment
    [HttpPost]
    public async Task<ActionResult> ProcessPayment(string paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            ViewBag.Status = "failed";
            ViewBag.Message = "Vui lòng chọn phương thức thanh toán.";
            return View("Index");
        }

        // Tạo mã đơn hàng duy nhất
        string orderId = Guid.NewGuid().ToString();

        if (paymentMethod == "bank_transfer")
        {
            // Mã ngân hàng và số tài khoản yêu cầu
            string bankCode = "ACB";
            string bankAccount = "35639567";

            // Tạo URL để chuyển hướng sang trang thanh toán của Pay2S với mã ngân hàng và tài khoản
            string pay2SRedirectUrl = _pay2SService.GetPaymentUrl(orderId, bankCode, bankAccount);

            // Chuyển hướng người dùng đến trang Pay2S để thực hiện thanh toán
            return Redirect(pay2SRedirectUrl);
        }
        else if (paymentMethod == "cash")
        {
            // Thanh toán tiền mặt, chuyển trạng thái đơn hàng sang "pending"
            ViewBag.Status = "pending";
            ViewBag.Message = "Đơn hàng của bạn đang chờ xử lý. Vui lòng thanh toán tiền mặt khi nhận hàng.";
            return View("PaymentPending");
        }

        ViewBag.Status = "failed";
        ViewBag.Message = "Phương thức thanh toán không hợp lệ.";
        return View("Index");
    }

    // GET: Checkout/PaymentSuccess
    public ActionResult PaymentSuccess(string orderId)
    {
        // Kiểm tra xem mã đơn hàng đã được thanh toán thành công chưa
        var paymentStatus = _pay2SService.CheckPaymentStatus(orderId);

        if (paymentStatus)
        {
            ViewBag.Message = "Thanh toán thành công! Cảm ơn bạn đã mua hàng.";
            return View("PaymentSuccess");
        }
        else
        {
            ViewBag.Message = "Thanh toán không thành công. Vui lòng thử lại hoặc liên hệ hỗ trợ.";
            return View("PaymentFailed");
        }
    }
}
