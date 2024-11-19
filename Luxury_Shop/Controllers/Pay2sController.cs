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

                    var response = await client.PostAsync(ApiUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        return $"{{\n  \"status\": false,\n  \"message\": \"API returned status {response.StatusCode}: {response.ReasonPhrase}\"\n}}";
                    }

                    var responseData = await response.Content.ReadAsStringAsync(); // Đọc dữ liệu phản hồi
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                return $"{{\n  \"status\": false,\n  \"message\": \"{ex.Message}\"\n}}";
            }
        }

        // Endpoint 1: Lấy dữ liệu giao dịch (JSON trả về)
        // URL: /Pay2s/TransactionHistory
        public async Task<ActionResult> TransactionHistory(string bankAccounts = null)
        {
            var endDate = DateTime.Now;
            var beginDate = endDate.AddDays(-7);

            var beginDateStr = beginDate.ToString("dd/MM/yyyy");
            var endDateStr = endDate.ToString("dd/MM/yyyy");

            var result = await FetchTransactions(beginDateStr, endDateStr, bankAccounts);

            return Content(result, "application/json");
        }

        // Endpoint 2: Cron job
        // URL: /Pay2s/Cron
        public async Task<ActionResult> Cron()
        {
            var endDate = DateTime.Now;
            var beginDate = endDate.AddDays(-7);

            var beginDateStr = beginDate.ToString("dd/MM/yyyy");
            var endDateStr = endDate.ToString("dd/MM/yyyy");

            var result = await FetchTransactions(beginDateStr, endDateStr);

            return Json(new
            {
                Success = true,
                Message = "Cron job executed successfully",
                Data = Newtonsoft.Json.JsonConvert.DeserializeObject(result)
            }, JsonRequestBehavior.AllowGet);
        }

        // Endpoint: /Pay2s/CheckInvoiceStatus
        public async Task<ActionResult> CheckInvoiceStatus(string invoiceNumber)
        {
            if (string.IsNullOrEmpty(invoiceNumber))
            {
                return Json(new { code = "400", message = "Invoice number is required" }, JsonRequestBehavior.AllowGet);
            }

            var endDate = DateTime.Now;
            var beginDate = endDate.AddDays(-7);

            var beginDateStr = beginDate.ToString("dd/MM/yyyy");
            var endDateStr = endDate.ToString("dd/MM/yyyy");

            var resultJson = await FetchTransactions(beginDateStr, endDateStr);

            try
            {
                if (string.IsNullOrEmpty(resultJson) || resultJson.Trim() == "[]")
                {
                    return Json(new
                    {
                        code = "400",
                        message = "No transactions found"
                    }, JsonRequestBehavior.AllowGet);
                }

                // Deserialize dữ liệu thành đối tượng dynamic
                var transactionData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resultJson);

                // Kiểm tra nếu không có giao dịch
                if (transactionData == null || transactionData.transactions == null || transactionData.transactions.Count == 0)
                {
                    return Json(new
                    {
                        code = "400",
                        message = "No transactions found"
                    }, JsonRequestBehavior.AllowGet);
                }

                var transactions = ((IEnumerable<dynamic>)transactionData.transactions).Cast<dynamic>();
                var transaction = transactions.FirstOrDefault(t => t.invoiceNumber == invoiceNumber);


                if (transaction == null)
                {
                    return Json(new
                    {
                        code = "400",
                        message = "Invoice not found"
                    }, JsonRequestBehavior.AllowGet);
                }

                var status = (string)transaction.status;

                switch (status)
                {
                    case "completed":
                        return Json(new
                        {
                            code = "200",
                            message = "Payment successful",
                            redirect_url = "/your-success-page"
                        }, JsonRequestBehavior.AllowGet);
                    case "pending":
                        return Json(new
                        {
                            code = "400",
                            message = "Invoice is pending"
                        }, JsonRequestBehavior.AllowGet);
                    case "cancelled":
                        return Json(new
                        {
                            code = "400",
                            message = "Invoice has been cancelled"
                        }, JsonRequestBehavior.AllowGet);
                    default:
                        return Json(new
                        {
                            code = "400",
                            message = "Unknown invoice status"
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = "500",
                    message = $"Error processing invoice status: {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
