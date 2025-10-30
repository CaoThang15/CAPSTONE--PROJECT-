using SMarket.Business.DTOs.AI;
using SMarket.Business.Services.Interfaces;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SMarket.DataAccess.Repositories;
using SMarket.DataAccess.Common;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.Business.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorRepository _vectorRepository;
        private readonly string _model;

        public AIService(HttpClient httpClient, IConfiguration config, IEmbeddingService embeddingService, IVectorRepository vectorRepository)
        {
            _embeddingService = embeddingService;
            _vectorRepository = vectorRepository;
            _httpClient = httpClient;
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new ArgumentNullException("Gemini:ApiKey", "Gemini API key is missing in configuration");
            _model = config["Gemini:ChatModel"] ?? "models/gemini-2.5-flash";
        }

        public async Task<string> GenerateProductDescriptionAsync(ProductSuggestionInput input)
        {
            string endpoint = $"https://generativelanguage.googleapis.com/v1/{_model}:generateContent?key={_apiKey}";

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

        public async Task<string> ChatbotAnswerUserAsync(string userMessage)
        {
            var qVec = await _embeddingService.CreateEmbeddingAsync(userMessage);

            var vectors = await _vectorRepository.SearchSimilarAsync(qVec, k: 5);

            var sb = new StringBuilder();
            foreach (var vector in vectors)
            {
                sb.AppendLine($"- (Id:{vector.Id}) - (Name:{vector.Name}) - (ProductId:{vector.ProductId}) - {vector.Price:N0} VND");
                sb.AppendLine($"{vector.Description}");
                sb.AppendLine();
            }

            var prompt = $@"
                Bạn là trợ lý bán hàng cho trang secondhand. Dưới đây là các sản phẩm liên quan:
                {sb}

                Người dùng hỏi: {userMessage}

                Hãy trả lời ngắn gọn, thân thiện, bằng tiếng Việt. Nếu có sản phẩm phù hợp, trả về productId để người dùng xem. Không tự bịa thông tin.
                ";

            var body = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
            };
            var json = JsonSerializer.Serialize(body);
            var url = $"https://generativelanguage.googleapis.com/v1/{_model}:generateContent?key={_apiKey}";
            var res = await _httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var text = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
                throw new Exception($"Gen error: {text}");

            using var doc = JsonDocument.Parse(text);
            var generated = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return generated ?? "";
        }
    }
}
