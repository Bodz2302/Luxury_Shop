using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Pay2SService
{
    private readonly string _apiUrl = "https://payment.pay2s.vn/v1/gateway/api/create";
    private readonly string _pay2sToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJ1c2VyIjoiMDkwMjUwNjA5OSIsImltZWkiOiI0MDE0NC1iZmNiNzJjMGIyMjczNjZiZy"; // Thay bằng token thực tế của bạn

    // Phương thức tạo yêu cầu thanh toán qua Pay2S
    public async Task<string> CreatePaymentRequest(string orderId, string bankCode, string bankAccount, decimal amount, string description)
    {
        using (var client = new HttpClient())
        {
            // Thiết lập tiêu đề yêu cầu
            client.DefaultRequestHeaders.Add("pay2s-token", _pay2sToken);

            // Tạo payload yêu cầu thanh toán
            var requestData = new
            {
                orderId = orderId,
                bankCode = bankCode,
                bankAccount = bankAccount,
                amount = amount,
                description = description,
                returnUrl = "https://yourwebsite.com/Checkout/PaymentSuccess",
                cancelUrl = "https://yourwebsite.com/Checkout/PaymentFailed"
            };

            string jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Gửi yêu cầu POST đến API
            HttpResponseMessage response = await client.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Chuyển đổi nội dung phản hồi thành JSON object
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseContent);

                // Trả về URL thanh toán từ phản hồi
                return jsonResponse["paymentUrl"]?.ToString();
            }
            else
            {
                // Xử lý lỗi nếu phản hồi không thành công
                throw new Exception($"Failed to create payment request. Status Code: {response.StatusCode}");
            }
        }
    }
}
