using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace Luxury_Shop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly string _apiUrl = "https://my.pay2s.vn/userapi/transactions";
        private readonly string _pay2sToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJ1c2VyIjoiMDkwMjUwNjA5OSIsImltZWkiOiI0MDE0NC1iZmNiNzJjMGIyMjczNjZiZy";

        // GET: Checkout
        public ActionResult Index()
        {
            return View();
        }

        // POST: Checkout/Confirm
        [HttpPost]
        public async Task<ActionResult> Confirm(string bankAccount, string beginDate, string endDate)
        {
            var response = await CheckTransaction(bankAccount, beginDate, endDate);
            if (response != null && response["status"].ToObject<bool>())
            {
                // Giao dịch thành công
                ViewBag.Status = "success";
                ViewBag.Message = response["messages"].ToString();
            }
            else
            {
                // Giao dịch thất bại
                ViewBag.Status = "failed";
                ViewBag.Message = response?["messages"]?.ToString() ?? "Lỗi không xác định";
            }
            return View("Index");
        }

        private async Task<JObject> CheckTransaction(string bankAccount, string beginDate, string endDate)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("pay2s-token", _pay2sToken);
                var requestContent = new JObject
                {
                    { "bankAccounts", bankAccount },
                    { "begin", beginDate },
                    { "end", endDate }
                };
                var content = new StringContent(requestContent.ToString(), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(responseBody);
                }
                return null;
            }
        }
    }
}
