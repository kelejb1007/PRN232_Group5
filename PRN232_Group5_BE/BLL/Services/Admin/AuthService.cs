using AutoMapper;
using BCrypt.Net;
using BLL.Services.Admin.Interfaces;
using DAL.DTOs.UserAccount;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories.Admin.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static DAL.DTOs.UserAccount.AuthDTO;

namespace BLL.Services.Admin
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepo, IMapper mapper, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            // 1. Tìm user theo Username và kiểm tra trạng thái hoạt động
            var user = await _userRepo.GetUserByUsernameAsync(request.Username);

            if (user == null) return null;

            // 2. Sử dụng BCrypt để kiểm tra mật khẩu
            // request.Password: Mật khẩu thô từ Client gửi lên
            // user.PasswordHash: Chuỗi đã mã hóa lưu trong DB
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid) return null;

            // 3. Mapping sang DTO để trả về (Bảo mật, không lộ PasswordHash)
            return GenerateToken(user);
        }



        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var userName = request.Username.Trim();
            var exists = await _userRepo.IsUserNameExistsAsync(userName);
            if (exists)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            UserRole role = string.IsNullOrWhiteSpace(request.Role) ? UserRole.Customer : UserRole.Admin;

            var userAccount = new UserAccount
            {
                FullName = request.FullName.Trim(),
                Username = userName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role
            };

            await _userRepo.AddAsync(userAccount);
            return GenerateToken(userAccount);
        }



        private AuthResponseDTO GenerateToken(UserAccount user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
            var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing");
            var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing");
            var expireMinutesText = jwtSection["ExpireMinutes"];

            var expireMinutes = 60;
            if (!string.IsNullOrWhiteSpace(expireMinutesText) && int.TryParse(expireMinutesText, out var parsed))
            {
                expireMinutes = parsed;
            }

            var expires = DateTime.UtcNow.AddMinutes(expireMinutes);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new AuthResponseDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAtUtc = expires,
                Username = user.Username,
                Role = user.Role.ToString()
            };
        }

    }
}
