//using BE.Models.DTOs.Auth;
//using BE.Models.DTOs;
//using BE.Models.Entities;
//using GymCoach.Api.Config;
//using MongoDB.Driver;
//using System.Security.Claims;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Text;

//namespace BE.Services
//{
//    public class AuthService
//    {
//        private readonly MongoDbContext _db;
//        private readonly IMongoCollection<User> _users;
//        private readonly string _jwtKey;
//        private readonly string _jwtIssuer;
//        private readonly string _jwtAudience;
//        private readonly int _jwtExpiryMinutes;

//        public AuthService(MongoDbContext db, IConfiguration config)
//        {
//            _db = db;
//            _users = _db.GetCollection<User>("Users");
//            _jwtKey = config["Jwt:Key"] ?? string.Empty;
//            _jwtIssuer = config["Jwt:Issuer"] ?? string.Empty;
//            _jwtAudience = config["Jwt:Audience"] ?? string.Empty;
//            _jwtExpiryMinutes = int.TryParse(config["Jwt:ExpiryMinutes"], out var m) ? m : 60;
//        }

//        public async Task<string> RegisterCustomerAsync(RegisterCustomerRequest req)
//        {
//            var exists = await _users.Find(u => u.Email.ToLower() == req.Email.ToLower()).FirstOrDefaultAsync();
//            if (exists != null) throw new InvalidOperationException("Email already in use.");

//            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);
//            var user = new User
//            {
//                FullName = req.FullName,
//                Email = req.Email,
//                PasswordHash = hash,
//                Role = "Customer",
//                GymId = req.GymId
//            };

//            await _users.InsertOneAsync(user);
//            return user.Id;
//        }

//        public async Task<string> RegisterManagerAsync(RegisterManagerRequest req)
//        {
//            var exists = await _users.Find(u => u.Email.ToLower() == req.Email.ToLower()).FirstOrDefaultAsync();
//            if (exists != null) throw new InvalidOperationException("Email already in use.");

//            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);
//            var user = new User
//            {
//                FullName = req.FullName,
//                Email = req.Email,
//                PasswordHash = hash,
//                Role = "Manager",
//                GymId = req.GymId
//            };

//            await _users.InsertOneAsync(user);
//            return user.Id;
//        }

//        public async Task<AuthResponse> LoginAsync(LoginRequest req)
//        {
//            var user = await _users.Find(u => u.Email.ToLower() == req.Email.ToLower()).FirstOrDefaultAsync();
//            if (user == null) throw new UnauthorizedAccessException();

//            var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
//            if (!ok) throw new UnauthorizedAccessException();

//            var token = CreateToken(user);
//            return new AuthResponse
//            {
//                Token = token,
//                UserId = user.Id,
//                Email = user.Email,
//                Role = user.Role
//            };
//        }

//        private string CreateToken(User user)
//        {
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//            var expires = DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes);

//            var claims = new List<Claim>
//            {
//                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
//                new Claim(JwtRegisteredClaimNames.Email, user.Email),
//                new Claim(ClaimTypes.Role, user.Role)
//            };

//            var token = new JwtSecurityToken(
//                issuer: _jwtIssuer,
//                audience: _jwtAudience,
//                claims: claims,
//                expires: expires,
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}