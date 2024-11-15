using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Luxury_Shop.Controllers
{
    public class TransactionController : Controller
    {
        private readonly Pay2SService _pay2SService;

        public TransactionController()
        {
            _pay2SService = new Pay2SService(); // Khởi tạo Pay2SService
        }

        // GET: Transaction/Index
        public ActionResult Index()
        {
            return View();
        }

        // POST: Transaction/Confirm
        [HttpPost]
        public async Task<ActionResult> Confirm(string bankAccount)
        {
            // Kiểm tra tài khoản ngân hàng
            if (string.IsNullOrWhiteSpace(bankAccount))
            {
                ViewBag.Status = "failed";
                ViewBag.Message = "Vui lòng nhập số tài khoản ngân hàng.";
                return View("Index");
            }

            // Tự động lấy ngày hiện tại
            string currentDate = DateTime.Now.ToString("MM/dd/yyyy");

            // Gọi API lấy thông tin giao dịch trong ngày hiện tại
            var response = await _pay2SService.GetTransactions(bankAccount, currentDate, currentDate);

            if (response != null && response["status"].ToObject<bool>())
            {
                // Giao dịch thành công
                ViewBag.Status = "success";
                ViewBag.Message = response["messages"].ToString();
                ViewBag.Transactions = response["transactions"]; // Hiển thị danh sách giao dịch
            }
            else
            {
                // Giao dịch thất bại
                ViewBag.Status = "failed";
                ViewBag.Message = response?["messages"]?.ToString() ?? "Lỗi không xác định";
            }

            return View("Index");
        }
    }
}
