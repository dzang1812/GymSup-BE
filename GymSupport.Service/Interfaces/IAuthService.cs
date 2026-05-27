using GymSupport.Repository.Models.DTOs.Auth;
using System.Threading.Tasks;

namespace GymSupport.Service.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterCustomerAsync(RegisterCustomerRequest req);
        Task<string> RegisterManagerAsync(RegisterManagerRequest req);
        Task<AuthResponse> LoginAsync(LoginRequest req);
    }
}
