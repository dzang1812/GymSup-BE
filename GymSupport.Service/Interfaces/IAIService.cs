using GymSupport.Repository.Models.DTOs.AIModel;

namespace GymSupport.Service.Interfaces;

public interface IAIService
{
    Task<ChatResponseDto> ChatAsync(string message);
}