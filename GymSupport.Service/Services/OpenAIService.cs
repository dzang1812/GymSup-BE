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
    private readonly IWorkoutPlanRepository _workoutRepository;
    private readonly IExerciseRepository _exerciseRepository;
    public OpenAIService(
        HttpClient httpClient,
        IConfiguration configuration,
        IChatRepository chatRepository,
        IWorkoutPlanRepository workoutRepository,
        IExerciseRepository exerciseRepository)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _chatRepository = chatRepository;
        _workoutRepository = workoutRepository;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<ChatResponseDto> ChatAsync(
    string userId,
    string message)
    {
        var apiKey =
            _configuration["OpenAI:ApiKey"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey);

        var history =
            await _chatRepository
                .GetRecentMessagesAsync(userId, 20);

        history.Reverse();

        var plans =
            await _workoutRepository
                .GetByUserIdAsync(userId);

        var exercises =
            await _exerciseRepository
                .GetAllAsync();

        var workoutInfo =
            JsonSerializer.Serialize(plans);

        var exerciseNames =
            string.Join(
                ", ",
                exercises.Select(x => x.Name));

        var messages = new List<object>();

        messages.Add(new
        {
            role = "system",
            content =
    $"""
Bạn là GymSupport AI Coach.

Nhiệm vụ:
- Chỉ trả lời các câu hỏi liên quan đến gym, fitness, cardio, bodybuilding, tăng cơ, giảm mỡ và dinh dưỡng.
- Trả lời bằng tiếng Việt.
- Với câu hỏi ngắn, trả lời ngắn gọn.
- Nếu người dùng muốn giải thích thêm thì mới trả lời chi tiết.
- Nếu câu hỏi không liên quan fitness hãy trả lời:
'Xin lỗi, tôi chỉ hỗ trợ các câu hỏi về tập luyện và dinh dưỡng.'

Danh sách bài tập hiện có:

{exerciseNames}

WorkoutPlan hiện tại:

{workoutInfo}

Nếu người dùng muốn cải thiện lịch tập:
- Chỉ đề xuất các bài tập trong danh sách trên.
- Không tạo bài tập mới.
- Không tạo ID mới.
- Có thể đề xuất thêm bài tập vào lịch hiện tại.
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

        var chatResponse =
            new ChatResponseDto
            {
                Response = aiResponse ?? ""
            };

        var firstPlan =
            plans.FirstOrDefault();

        if (firstPlan != null)
        {
            foreach (var exercise in exercises)
            {
                if (string.IsNullOrWhiteSpace(aiResponse))
                    continue;

                if (!aiResponse.Contains(
                        exercise.Name,
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                var session =
                    firstPlan.Sessions
                        .FirstOrDefault();

                if (session == null)
                    continue;

                chatResponse.Suggestions.Add(
                    new AISuggestionDto
                    {
                        Action = "add_exercise",

                        PlanId = firstPlan.Id,

                        SessionId = session.Id,

                        ExerciseId = exercise.Id,

                        Sets = 4,

                        Reps = "8-12",

                        Notes = "AI Recommendation"
                    });
            }
        }

        return chatResponse;
    }
}