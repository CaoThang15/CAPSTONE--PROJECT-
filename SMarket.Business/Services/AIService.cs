using SMarket.Business.DTOs.AI;
using SMarket.Business.Services.Interfaces;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SMarket.Business.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new ArgumentNullException("Gemini:ApiKey", "Gemini API key is missing in configuration");
        }

        public async Task<string> GenerateProductDescriptionAsync(ProductSuggestionInput input)
        {
            string endpoint = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            string prompt = $@"
                Hãy viết một mô tả sản phẩm hấp dẫn và trung thực cho sản phẩm second-hand hoặc mới tùy theo thông tin.
                Thông tin sản phẩm:
                - Danh mục: {input.CategoryName}
                - Tên sản phẩm: {input.Name}
                - Giá bán: {input.Price} VND
                - Mô tả ban đầu: {input.Description}
                - Ghi chú thêm: {input.Note}
                - Tình trạng: {(input.IsNew ? "Sản phẩm mới" : "Đã qua sử dụng")}
                - Thuộc tính: {string.Join(", ", input.Properties.Select(p => $"{p.Name}: {p.Value}"))}

                Yêu cầu:
                1. Viết mô tả ngắn gọn, thu hút, rõ ràng, phù hợp cho người mua tại Việt Nam.
                2. Nếu sản phẩm đã qua sử dụng, cần nói rõ tình trạng nhưng vẫn hấp dẫn.
                3. Kết thúc bằng lời kêu gọi hành động như 'Liên hệ ngay' hoặc 'Mua ngay hôm nay'.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            string responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Gemini API error: {response.StatusCode} - {responseText}");

            using var doc = JsonDocument.Parse(responseText);
            string generatedText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return generatedText;
        }
    }
}
