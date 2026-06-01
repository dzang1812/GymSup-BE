using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GymSupport.Repository.Models.DTOs.AIModel;
using GymSupport.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GymSupport.Service.Services;

public class OpenAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAIService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ChatResponseDto> ChatAsync(
        string message)
    {
        var apiKey =
            _configuration["OpenAI:ApiKey"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey);

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content =
                        "Bạn là huấn luyện viên gym chuyên về tập luyện, dinh dưỡng, tăng cơ và giảm mỡ." +
                        "Nhiệm vụ:\r\n- Chỉ trả lời các câu hỏi liên quan đến gym, fitness, bodybuilding, cardio," +
                        " giảm cân, tăng cơ, dinh dưỡng thể thao và sức khỏe tập luyện.\r\n- Trả lời ngắn gọn, súc tích, với các câu hỏi ngắn chỉ tra lời ngắn gọn, và hỏi họ có muốn rõ hơn không thì mới trả lời dài tối đa 150 từ.\r\n- " +
                        "Trả lời bằng tiếng Việt.\r\n- Nếu câu hỏi không liên quan đến fitness, hãy trả lời:\r\n'Xin lỗi, tôi chỉ hỗ trợ các câu hỏi về tập luyện và dinh dưỡng.'\r\n- Không trả lời các chủ đề chính trị, tôn giáo, lập trình, hack, tài chính hoặc nội dung ngoài fitness.\r\n"
                },
                new
                {
                    role = "user",
                    content = message
                }
            },
            temperature = 0.7
        };

        var json =
            JsonSerializer.Serialize(requestBody);

        var response =
            await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync();

            throw new Exception(
                $"OpenAI Error: {error}");
        }

        var result =
            await response.Content.ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(result);

        var aiResponse =
            document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        return new ChatResponseDto
        {
            Response = aiResponse ?? ""
        };
    }
}