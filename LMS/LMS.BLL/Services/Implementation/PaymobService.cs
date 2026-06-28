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
            // 2. Register Order => Updated types and added empty items array
            var orderRequest = new
            {
                auth_token = authToken,
                delivery_needed = false,  // changed from "false" to false (boolean)
                amount_cents = amountCents, // changed from amountCents.ToString() to numeric integer
                currency = "EGP",
                merchant_order_id = dbPaymentId, // Ties Paymob order to your DB
                items = Array.Empty<object>()   // added empty items array
            };
            var orderResponse = await PostAsync("https://accept.paymob.com/api/ecommerce/orders", orderRequest);
            var paymobOrderId = orderResponse.GetProperty("id").GetInt32();
            // 3. Get Payment Key (Updated amount_cents and order_id to integers)
            var paymentKeyRequest = new
            {
                auth_token = authToken,
                amount_cents = amountCents,  // changed to numeric integer
                expiration = 3600,
                order_id = paymobOrderId,   // changed from paymobOrderId.ToString() to numeric integer
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

        // Safe property extractor that checks if property exists before retrieving it to avoid KeyNotFoundExceptions
        private string GetSafeHmacValue(JsonElement parent, string propertyName)
        {
            if (parent.ValueKind == JsonValueKind.Object && parent.TryGetProperty(propertyName, out var element))
            {
                if (element.ValueKind == JsonValueKind.True) return "true";
                if (element.ValueKind == JsonValueKind.False) return "false";
                if (element.ValueKind == JsonValueKind.Null) return "";
                return element.ToString() ?? "";
            }
            return "";
        }

        public bool VerifyHmac(JsonElement payload, string receivedHmac)
        {
            var secret = Environment.GetEnvironmentVariable("PAYMOB_HMAC_SECRET") ?? _config["PAYMOB_HMAC_SECRET"] ?? throw new InvalidOperationException("PAYMOB_HMAC_SECRET is not configured");
            var obj = payload.GetProperty("obj");

            // 💡 Safely extract sub-objects to prevent KeyNotFoundException crashes on non-card transactions
            var sourceData = obj.TryGetProperty("source_data", out var sd) ? sd : default;
            var orderObj = obj.TryGetProperty("order", out var ord) ? ord : default;

            // Paymob requires exact alphabetical concatenation of these fields
            string concatenatedString =
                GetSafeHmacValue(obj, "amount_cents") +
                GetSafeHmacValue(obj, "created_at") +
                GetSafeHmacValue(obj, "currency") +
                GetSafeHmacValue(obj, "error_occured") +
                GetSafeHmacValue(obj, "has_parent_transaction") +
                GetSafeHmacValue(obj, "id") +
                GetSafeHmacValue(obj, "integration_id") +
                GetSafeHmacValue(obj, "is_3d_secure") +
                GetSafeHmacValue(obj, "is_auth") +
                GetSafeHmacValue(obj, "is_capture") +
                GetSafeHmacValue(obj, "is_refunded") +
                GetSafeHmacValue(obj, "is_standalone_payment") +
                GetSafeHmacValue(obj, "is_voided") +
                (orderObj.ValueKind == JsonValueKind.Object ? GetSafeHmacValue(orderObj, "id") : "") +
                GetSafeHmacValue(obj, "owner") +
                GetSafeHmacValue(obj, "pending") +
                (sourceData.ValueKind == JsonValueKind.Object ? GetSafeHmacValue(sourceData, "pan") : "") +
                (sourceData.ValueKind == JsonValueKind.Object ? GetSafeHmacValue(sourceData, "sub_type") : "") +
                (sourceData.ValueKind == JsonValueKind.Object ? GetSafeHmacValue(sourceData, "type") : "") +
                GetSafeHmacValue(obj, "success");

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

            // Read the error response if it fails
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request to {url} failed with status {response.StatusCode}. Response: {errorBody}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(responseString).RootElement;
        }
    }
}
