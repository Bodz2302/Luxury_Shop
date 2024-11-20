using Luxury_Shop.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Luxury_Shop.Controllers
{
    public class TelegramController : Controller
    {
        private LuxuryEntities1 database = new LuxuryEntities1();

        // Thay thế bằng Token Bot và Chat ID của bạn
        private readonly string telegramToken = "8148584705:AAFM8WgfHftwUeXuQfdKKUs2Faig-Dia2T4";
        private readonly string chatId = "-4570507432";

        [HttpPost]
        public async Task<ActionResult> NotifyOrder(int orderId)
        {
            try
            {
                // Lấy thông tin đơn hàng từ cơ sở dữ liệu
                var order = database.Orders.FirstOrDefault(o => o.OrderID == orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Order not found." });
                }

                // Ghi log thông tin đơn hàng
                Console.WriteLine($"OrderID: {order.OrderID}, Customer: {order.FullName}, Amount: {order.TotalAmount}");

                // Tạo nội dung thông báo
                string message = $"📦 *CÓ ĐƠN ĐẶT HÀNG MỚI!*\n\n" +
                                 $"MÃ ĐƠN HÀNG: {order.OrderID}\n" +
                                 $"KHÁCH HÀNG: {order.FullName}\n" +
                                 $"TỔNG ĐƠN HÀNG: {order.TotalAmount} USD\n" +
                                 $"NGÀY TẠO: {order.OrderDate:dd/MM/yyyy}";

                // Gửi thông báo qua Telegram
                bool result = await SendTelegramMessage(message);

                if (result)
                {
                    return Json(new { success = true, message = "Notification sent successfully!" });
                }

                return Json(new { success = false, message = "Failed to send notification." });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        private async Task<bool> SendTelegramMessage(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // URL Telegram Bot API
                    string url = $"https://api.telegram.org/bot{telegramToken}/sendMessage";

                    // Payload (Nội dung JSON)
                    var payload = new
                    {
                        chat_id = chatId,
                        text = message,
                        parse_mode = "Markdown" // Định dạng Markdown cho tin nhắn
                    };

                    // Chuyển payload thành JSON
                    string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

                    // Gửi HTTP POST đến Telegram API
                    var response = await client.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

                    // Kiểm tra kết quả và ghi chi tiết response
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Telegram API Response: {responseBody}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi
                    Console.WriteLine($"Error sending Telegram message: {ex.Message}");
                    return false;
                }
            }
        }

    }
}
