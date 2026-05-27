using GymSupport.Repository.Models.DTOs.Auth;
using GymSupport.Repository.Models.Entities;
using GymSupport.Repository.Interfaces;
using GymSupport.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace GymSupport.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository repo, ITokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        public async Task<string> RegisterCustomerAsync(RegisterCustomerRequest req)
        {
            var exists = await _repo.GetByEmailAsync(req.Email);
            if (exists != null) throw new InvalidOperationException("Email already in use.");

            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                PasswordHash = hash,
                Role = "Customer",
                GymId = req.GymId
            };

            await _repo.CreateAsync(user);
            return user.Id;
        }

        public async Task<string> RegisterManagerAsync(RegisterManagerRequest req)
        {
            var exists = await _repo.GetByEmailAsync(req.Email);
            if (exists != null) throw new InvalidOperationException("Email already in use.");

            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                PasswordHash = hash,
                Role = "Manager",
                GymId = req.GymId
            };

            await _repo.CreateAsync(user);
            return user.Id;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req)
        {
            var user = await _repo.GetByEmailAsync(req.Email);
            if (user == null) throw new UnauthorizedAccessException();

            var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!ok) throw new UnauthorizedAccessException();

            var token = _tokenService.CreateToken(user);
            return new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
