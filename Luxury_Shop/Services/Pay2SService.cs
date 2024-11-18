using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Pay2SService
{
    private readonly string _apiUrl = "https://payment.pay2s.vn/v1/gateway/api/create"; // API URL Pay2S
    private readonly string _pay2sToken = "your-pay2s-token"; // Thay bằng token thực tế của bạn

    /// <summary>
    /// Tạo thanh toán qua Pay2S
    /// </summary>
    /// <param name="orderId">Mã đơn hàng</param>
    /// <param name="bankCode">Mã ngân hàng</param>
    /// <param name="bankAccount">Số tài khoản ngân hàng</param>
    /// <param name="amount">Số tiền cần thanh toán</param>
    /// <param name="description">Mô tả giao dịch</param>
    /// <returns>Phản hồi JSON từ API</returns>
    public async Task<JObject> CreatePayment(string orderId, string bankCode, string bankAccount, decimal amount, string description)
    {
        try
        {
            using (var client = new HttpClient())
            {
                // Thêm tiêu đề xác thực
                client.DefaultRequestHeaders.Add("pay2s-token", _pay2sToken);

                // Tạo payload yêu cầu
                var requestData = new
                {
                    orderId = orderId,
                    bankCode = bankCode,
                    bankAccount = bankAccount,
                    amount = amount,
                    description = description,
                    returnUrl = "https://yourwebsite.com/Checkout/PaymentSuccess", // URL trả về khi thanh toán thành công
                    cancelUrl = "https://yourwebsite.com/Checkout/PaymentFailed",   // URL trả về khi người dùng hủy
                    ipnUrl = "https://yourwebsite.com/Checkout/PaymentIPN"         // URL nhận kết quả qua IPN
                };

                // Chuyển đổi payload thành JSON
                string jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Gửi POST request đến API Pay2S
                HttpResponseMessage response = await client.PostAsync(_apiUrl, content);

                // Kiểm tra trạng thái phản hồi
                if (response.IsSuccessStatusCode)
                {
                    // Đọc nội dung phản hồi
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(responseContent); // Chuyển đổi thành đối tượng JSON
                }
                else
                {
                    // Xử lý khi API trả lỗi
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create payment. Status: {response.StatusCode}, Reason: {response.ReasonPhrase}, Content: {errorContent}");
                }
            }
        }
        catch (Exception ex)
        {
            // Ghi log lỗi (tùy theo hệ thống của bạn)
            Console.WriteLine($"Error while creating payment: {ex.Message}");
            throw;
        }
    }
}
