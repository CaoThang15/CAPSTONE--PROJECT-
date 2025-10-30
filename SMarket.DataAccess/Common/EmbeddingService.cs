using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SMarket.DataAccess.Common
{
    public interface IEmbeddingService
    {
        Task<float[]> CreateEmbeddingAsync(string text);
    }

    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;

        public EmbeddingService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new ArgumentNullException("Gemini:ApiKey", "Gemini API key is missing in configuration");
            _model = config["Gemini:EmbeddingModel"] ?? "models/text-embedding-004";
        }

        public async Task<float[]> CreateEmbeddingAsync(string text)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/{_model}:embedContent?key={_apiKey}";

            // ✅ Đúng format JSON cho Gemini
            var body = new
            {
                model = _model,
                content = new
                {
                    parts = new[]
                    {
                        new { text = text }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var res = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var txt = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception($"Embedding error: {txt}");

            using var doc = JsonDocument.Parse(txt);

            // ✅ Cấu trúc response đúng với Gemini embedding API:
            // {
            //   "embedding": { "values": [ ... ] }
            // }
            var values = doc.RootElement
                .GetProperty("embedding")
                .GetProperty("values")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            return values;
        }
    }

}