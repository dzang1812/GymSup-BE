using GymSupport.Repository.Models.DTOs.AIModel;
using GymSupport.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymSupport.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(
        IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] ChatRequestDto dto)
    {
        var response =
            await _aiService.ChatAsync(
                dto.Message);

        return Ok(response);
    }
}