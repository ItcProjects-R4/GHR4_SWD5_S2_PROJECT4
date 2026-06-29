using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using LMS.BLL.Services.Interfaces;

namespace LMS.BLL.Services.Implementation
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PaymobService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetPaymentKeyAsync(decimal amount, string studentEmail, string studentFirstName, string studentLastName, string dbPaymentId)
        {
            var apiKey = Environment.GetEnvironmentVariable("PAYMOB_API_KEY") ?? _config["PAYMOB_API_KEY"];
            var integrationId = int.Parse(Environment.GetEnvironmentVariable("PAYMOB_INTEGRATION_ID") ?? _config["PAYMOB_INTEGRATION_ID"]);
            var amountCents = (int)(amount * 100);

            // 1. Get Auth Token
            var authResponse = await PostAsync("https://accept.paymob.com/api/auth/tokens", new { api_key = apiKey });
            var authToken = authResponse.GetProperty("token").GetString();

            // 2. Register Order
            var orderRequest = new
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = amountCents.ToString(),
                currency = "EGP",
                merchant_order_id = dbPaymentId // Ties Paymob order to your DB
            };
            var orderResponse = await PostAsync("https://accept.paymob.com/api/ecommerce/orders", orderRequest);
            var paymobOrderId = orderResponse.GetProperty("id").GetInt32();

            // 3. Get Payment Key
            var paymentKeyRequest = new
            {
                auth_token = authToken,
                amount_cents = amountCents.ToString(),
                expiration = 3600,
                order_id = paymobOrderId.ToString(),
                billing_data = new
                {
                    apartment = "NA",
                    email = studentEmail,
                    floor = "NA",
                    first_name = studentFirstName,
                    street = "NA",
                    building = "NA",
                    phone_number = "01000000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = studentLastName,
                    state = "NA"
                },
                currency = "EGP",
                integration_id = integrationId
            };

            var keyResponse = await PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", paymentKeyRequest);
            return keyResponse.GetProperty("token").GetString();
        }

        private string GetHmacValue(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.True) return "true";
            if (element.ValueKind == JsonValueKind.False) return "false";
            if (element.ValueKind == JsonValueKind.Null) return "";
            return element.ToString() ?? "";
        }

        public bool VerifyHmac(JsonElement payload, string receivedHmac)
        {
            var secret = Environment.GetEnvironmentVariable("PAYMOB_HMAC_SECRET") ?? _config["PAYMOB_HMAC_SECRET"] ?? throw new InvalidOperationException("PAYMOB_HMAC_SECRET is not configured");
            var obj = payload.GetProperty("obj");

            // Paymob requires exact alphabetical concatenation of these fields
            string concatenatedString =
                GetHmacValue(obj.GetProperty("amount_cents")) +
                GetHmacValue(obj.GetProperty("created_at")) +
                GetHmacValue(obj.GetProperty("currency")) +
                GetHmacValue(obj.GetProperty("error_occured")) +
                GetHmacValue(obj.GetProperty("has_parent_transaction")) +
                GetHmacValue(obj.GetProperty("id")) +
                GetHmacValue(obj.GetProperty("integration_id")) +
                GetHmacValue(obj.GetProperty("is_3d_secure")) +
                GetHmacValue(obj.GetProperty("is_auth")) +
                GetHmacValue(obj.GetProperty("is_capture")) +
                GetHmacValue(obj.GetProperty("is_refunded")) +
                GetHmacValue(obj.GetProperty("is_standalone_payment")) +
                GetHmacValue(obj.GetProperty("is_voided")) +
                GetHmacValue(obj.GetProperty("order").GetProperty("id")) +
                GetHmacValue(obj.GetProperty("owner")) +
                GetHmacValue(obj.GetProperty("pending")) +
                GetHmacValue(obj.GetProperty("source_data").GetProperty("pan")) +
                GetHmacValue(obj.GetProperty("source_data").GetProperty("sub_type")) +
                GetHmacValue(obj.GetProperty("source_data").GetProperty("type")) +
                GetHmacValue(obj.GetProperty("success"));

            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var hashBytes = Encoding.UTF8.GetBytes(concatenatedString);

            using var hmac = new HMACSHA512(keyBytes);
            var computedHash = hmac.ComputeHash(hashBytes);
            var computedHmac = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

            return computedHmac == receivedHmac.ToLower();
        }

        private async Task<JsonElement> PostAsync(string url, object data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(responseString).RootElement;
        }
    }
}