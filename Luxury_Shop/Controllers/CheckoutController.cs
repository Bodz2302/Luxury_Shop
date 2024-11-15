using Luxury_Shop.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using System.Linq;

public class CheckoutController : Controller
{
    private readonly LuxuryEntities1 database = new LuxuryEntities1();

    // GET: Checkout/Index
    public ActionResult Index()
    {
        Cart cart = Session["Cart"] as Cart;
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("ShowCart", "ShoppingCart");
        }

        // Tạo CheckoutViewModel để truyền vào view
        var viewModel = new CheckoutViewModel
        {
            CartItems = cart.Items,
            TotalAmount = GetTotalAmount(),
        };

        return View(viewModel);
    }


    // Phương thức xử lý thanh toán (POST)
    public enum PaymentMethod
    {
        BankTransfer,
        Cash
    }

    [HttpPost]
    public ActionResult ProcessPayment(string paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            return Json(new { status = "failed", message = "Vui lòng chọn phương thức thanh toán." });
        }

        decimal totalAmount = GetTotalAmount();
        if (totalAmount <= 0)
        {
            return Json(new { status = "failed", message = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán." });
        }

        string orderId = Guid.NewGuid().ToString();

        if (paymentMethod == "bank_transfer")
        {
            // Chuyển sang trang thanh toán với mã QR
            return RedirectToAction("Transfer", new { orderId = orderId, totalAmount = totalAmount });
        }
        else if (paymentMethod == "cash")
        {
            return Json(new { status = "pending", message = "Đơn hàng đang chờ xử lý. Vui lòng thanh toán tiền mặt khi nhận hàng." });
        }

        return Json(new { status = "failed", message = "Phương thức thanh toán không hợp lệ." });
    }

    public async Task<ActionResult> CheckPaymentStatus(string orderId, decimal totalAmount)
    {
        // Gọi API kiểm tra trạng thái thanh toán
        string apiUrl = $"https://api.sieuthicode.net/historyapiacbv2/777a5476ec426a80d379b348d097f03d";
        var response = await GetBankTransactionHistory(apiUrl);

        if (response.status == "success")
        {
            var transactions = response.transactions as List<Transaction>;
            bool isPaymentValid = transactions != null && transactions.Exists(t => t.Amount == totalAmount && t.Description.Contains(orderId));

            if (isPaymentValid)
            {
                return Json(new { status = "success", message = "Thanh toán thành công!" });
            }
        }

        return Json(new { status = "failed", message = "Chưa tìm thấy giao dịch thanh toán." });
    }


    // Phương thức tính tổng số tiền trong giỏ hàng
    private decimal GetTotalAmount()
    {
        // Lấy giỏ hàng từ session
        Cart cart = Session["Cart"] as Cart;
        if (cart == null || cart.Items == null || cart.Items.Count == 0)
        {
            return 0;
        }

        // Tính tổng số tiền của giỏ hàng
        return cart.Items.Sum(item => item.Product.OriginalPrice * item.Quantity);
    }

    // Phương thức gọi API lấy lịch sử giao dịch ngân hàng
    private async Task<dynamic> GetBankTransactionHistory(string apiUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (responseData.status == "success" && responseData.transactions != null)
                    {
                        var transactions = JsonConvert.DeserializeObject<List<Transaction>>(responseData.transactions.ToString());
                        return new { status = "success", transactions = transactions };
                    }
                    else
                    {
                        return new { status = "failed", message = "Không thể lấy lịch sử giao dịch." };
                    }
                }
                else
                {
                    return new { status = "failed", message = "Không thể lấy lịch sử giao dịch." };
                }
            }
            catch (Exception ex)
            {
                return new { status = "failed", message = $"Lỗi khi gọi API: {ex.Message}" };
            }
        }
    }
}
