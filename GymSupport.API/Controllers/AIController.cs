using GymSupport.Repository.Interfaces;
using GymSupport.Repository.Models.DTOs.AIModel;
using GymSupport.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymSupport.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IChatRepository _chatRepository;
    public AIController(
        IAIService aiService,
        IChatRepository chatRepository)
    {
        _aiService = aiService;
        _chatRepository = chatRepository;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] ChatRequestDto dto)
    {
        var result =
            await _aiService.ChatAsync(
                dto.UserId,
                dto.Message);

        return Ok(result);
    }

    [HttpGet("history/{userId}")]
    public async Task<IActionResult> GetHistory(
    string userId)
    {
        var messages =
            await _chatRepository
                .GetByUserIdAsync(userId);

        var result =
            messages.Select(x =>
                new ChatHistoryDto
                {
                    Role = x.Role,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt
                });

        return Ok(result);
    }
}