using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Luxury_Shop.Controllers
{
    public class Pay2sController : Controller
    {
            private const string ApiUrl = "https://my.pay2s.vn/userapi/transactions";
            private const string ApiToken = "NjNmYjRjNjZiMjE0ZjkzMzIxZDNhODEzZTRkZDcyYThmYzlhMWFlZDMwOGIzNzdkYTYwZDFhZWVhMzRmMjhhNw=="; // Thay bằng token thực tế của bạn

        // Hàm hỗ trợ lấy dữ liệu từ API
        public async Task<string> FetchTransactions(string beginDate, string endDate, string bankAccounts = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("pay2s-token", ApiToken); // Thêm token vào header

                    var requestBody = new
                    {
                        bankAccounts = bankAccounts,
                        begin = beginDate,
                        end = endDate
                    };

                    var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                    // Tạo StringContent với header "Content-Type: application/json"
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(ApiUrl, content); // Gửi yêu cầu POST với nội dung
                    response.EnsureSuccessStatusCode();

                    var responseData = await response.Content.ReadAsStringAsync(); // Đọc dữ liệu phản hồi
                    return responseData;
                }

            }
            catch (Exception ex)
            {
                return $"{{\"status\": false, \"message\": \"{ex.Message}\"}}";
            }
        }


        // Endpoint 1: Lấy dữ liệu giao dịch (JSON trả về)
        // URL: /Pay2s/TransactionHistory
        public async Task<ActionResult> TransactionHistory(string bankAccounts = null)
            {
                // Tính ngày bắt đầu và ngày kết thúc (7 ngày gần nhất)
                var endDate = DateTime.Now;
                var beginDate = endDate.AddDays(-7);

                var beginDateStr = beginDate.ToString("dd/MM/yyyy");
                var endDateStr = endDate.ToString("dd/MM/yyyy");

                // Lấy dữ liệu từ API
                var result = await FetchTransactions(beginDateStr, endDateStr, bankAccounts);

                // Trả về JSON
                return Content(result, "application/json");
            }

            // Endpoint 2: Cron job
            // URL: /Pay2s/Cron
            public async Task<ActionResult> Cron()
            {
                // Tính ngày bắt đầu và ngày kết thúc (7 ngày gần nhất)
                var endDate = DateTime.Now;
                var beginDate = endDate.AddDays(-7);

                var beginDateStr = beginDate.ToString("dd/MM/yyyy");
                var endDateStr = endDate.ToString("dd/MM/yyyy");

                // Lấy dữ liệu từ API
                var result = await FetchTransactions(beginDateStr, endDateStr);

                // Ở đây bạn có thể thêm logic lưu dữ liệu vào database nếu cần thiết
                // Ví dụ: Parse JSON và lưu các giao dịch mới vào database

                return Json(new
                {
                    Success = true,
                    Message = "Cron job executed successfully",
                    Data = result
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }