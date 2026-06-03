using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GymSupport.Repository.Interfaces;
using GymSupport.Repository.Models.DTOs.AIModel;
using GymSupport.Repository.Models.Entities;
using GymSupport.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GymSupport.Service.Services;

public class OpenAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IChatRepository _chatRepository;

    public OpenAIService(
        HttpClient httpClient,
        IConfiguration configuration,
        IChatRepository chatRepository)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _chatRepository = chatRepository;
    }

    public async Task<ChatResponseDto> ChatAsync(
        string userId,
        string message)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey);

        var history =
            await _chatRepository
                .GetRecentMessagesAsync(userId, 20);

        history.Reverse();

        var messages = new List<object>();

        messages.Add(new
        {
            role = "system",
            content =
                """
                Bạn là GymSupport AI Coach.

                Nhiệm vụ:
                - Chỉ trả lời các câu hỏi liên quan đến gym, fitness, bodybuilding, cardio, giảm cân, tăng cơ, dinh dưỡng thể thao và sức khỏe tập luyện.
                - Trả lời bằng tiếng Việt.
                - Với câu hỏi ngắn, trả lời ngắn gọn.
                - Nếu người dùng muốn chi tiết hơn thì mới giải thích sâu.
                - Nếu câu hỏi không liên quan fitness, trả lời:
                  'Xin lỗi, tôi chỉ hỗ trợ các câu hỏi về tập luyện và dinh dưỡng.'
                - Không trả lời chính trị, hack, lập trình, tài chính, tôn giáo.
                - Hãy ghi nhớ ngữ cảnh cuộc trò chuyện trước đó.
                """
        });

        foreach (var item in history)
        {
            messages.Add(new
            {
                role = item.Role,
                content = item.Content
            });
        }

        messages.Add(new
        {
            role = "user",
            content = message
        });

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages,
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

        await _chatRepository.CreateAsync(
            new ChatMessage
            {
                UserId = userId,
                Role = "user",
                Content = message
            });

        await _chatRepository.CreateAsync(
            new ChatMessage
            {
                UserId = userId,
                Role = "assistant",
                Content = aiResponse ?? ""
            });

        return new ChatResponseDto
        {
            Response = aiResponse ?? ""
        };
    }
}