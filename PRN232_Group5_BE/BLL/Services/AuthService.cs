using AutoMapper;
using BCrypt.Net;
using BLL.Services.Interfaces;
using DAL.DTOs.UserAccount;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Repositories.User.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using static DAL.DTOs.UserAccount.AuthDTO;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public AuthService(IUserRepository userRepo, IMapper mapper, IConfiguration configuration, IEmailService emailService, IMemoryCache cache)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _cache = cache;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            // 1. Tìm user theo Username và kiểm tra trạng thái hoạt động
            var user = await _userRepo.GetUserByUsernameAsync(request.Username);

            if (user == null || !user.IsActive) return null;

            // 2. Sử dụng BCrypt để kiểm tra mật khẩu
            // request.Password: Mật khẩu thô từ Client gửi lên
            // user.PasswordHash: Chuỗi đã mã hóa lưu trong DB
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid) return null;

            // 3. Mapping sang DTO để trả về (Bảo mật, không lộ PasswordHash)
            return GenerateToken(user);
        }



        public async Task<bool> RegisterAsync(RegisterRequestDTO request)
        {
            var userName = request.Username.Trim();
            var email = request.Email?.Trim() ?? string.Empty;

            var cacheKey = $"otp_limit_{email.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out int attempts))
            {
                if (attempts >= 5)
                {
                    throw new InvalidOperationException("Bạn đã vượt quá 5 lần yêu cầu mã xác thực trong 2 giờ. Vui lòng thử lại sau.");
                }
                _cache.Set(cacheKey, attempts + 1, TimeSpan.FromHours(2));
            }
            else
            {
                _cache.Set(cacheKey, 1, TimeSpan.FromHours(2));
            }

            UserRole role = string.IsNullOrWhiteSpace(request.Role) ? UserRole.Customer : UserRole.Admin;
            var otpCode = new Random().Next(100000, 999999).ToString();

            var existingUser = await _userRepo.GetUserByUsernameAsync(userName);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Tên đăng nhập đã tồn tại.");
            }

            var existingEmail = await _userRepo.GetUserByEmailAsync(email);
            if (existingEmail != null)
            {
                throw new InvalidOperationException("Email đã tồn tại.");
            }

            // Save the entire registration request along with the OTP to session (cache) instead of DB
            var pendingReg = new Tuple<RegisterRequestDTO, string, UserRole>(request, otpCode, role);
            _cache.Set($"pending_reg_{email.ToLower()}", pendingReg, TimeSpan.FromMinutes(2));

            await _emailService.SendEmailAsync(email, "Xác nhận đăng ký tài khoản", $"Mã xác nhận (OTP) của bạn là: {otpCode}\nMã có hiệu lực trong 2 phút.");

            return true;
        }

        public async Task<bool> VerifyEmailAsync(VerifyEmailRequestDTO request)
        {
            var email = request.Email?.Trim() ?? string.Empty;
            var token = request.Token?.Trim() ?? string.Empty;

            var cacheKey = $"pending_reg_{email.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out Tuple<RegisterRequestDTO, string, UserRole>? cachedData) && cachedData != null)
            {
                if (cachedData.Item2 == token)
                {
                    // Check again if username or email was taken in the meantime
                    var existingUser = await _userRepo.GetUserByUsernameAsync(cachedData.Item1.Username.Trim());
                    if (existingUser != null) return false;

                    var userAccount = new UserAccount
                    {
                        FullName = cachedData.Item1.FullName?.Trim() ?? string.Empty,
                        Username = cachedData.Item1.Username.Trim(),
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(cachedData.Item1.Password),
                        Email = email,
                        Role = cachedData.Item3,
                        IsActive = true
                    };

                    await _userRepo.AddAsync(userAccount);
                    _cache.Remove(cacheKey);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
        {
            var email = request.Email.Trim();
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null || !user.IsActive) return false;

            var cacheKey = $"reset_limit_{email.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out int attempts))
            {
                if (attempts >= 5)
                {
                    throw new InvalidOperationException("Bạn đã vượt quá 5 lần yêu cầu đặt lại mật khẩu trong 2 giờ.");
                }
                _cache.Set(cacheKey, attempts + 1, TimeSpan.FromHours(2));
            }
            else
            {
                _cache.Set(cacheKey, 1, TimeSpan.FromHours(2));
            }

            var otpCode = new Random().Next(100000, 999999).ToString();
            _cache.Set($"reset_pwd_{email.ToLower()}", otpCode, TimeSpan.FromMinutes(2));

            await _emailService.SendEmailAsync(email, "Khôi phục mật khẩu", $"Mã xác nhận (OTP) để khôi phục mật khẩu của bạn là: {otpCode}\nMã có hiệu lực trong 2 phút.");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            var email = request.Email.Trim();
            var token = request.Token.Trim();

            var cacheKey = $"reset_pwd_{email.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out string? cachedToken) && cachedToken == token)
            {
                var user = await _userRepo.GetUserByEmailAsync(email);
                if (user == null || !user.IsActive) return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _userRepo.UpdateAsync(user);

                _cache.Remove(cacheKey);
                return true;
            }
            return false;
        }

        public async Task<UserProfileDTO?> GetProfileAsync(string username)
        {
            var user = await _userRepo.GetUserByUsernameAsync(username);
            if (user == null || !user.IsActive) return null;

            return new UserProfileDTO
            {
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role.ToString()
            };
        }

        public async Task<bool> UpdateProfileAsync(string username, UpdateProfileRequestDTO request)
        {
            var user = await _userRepo.GetUserByUsernameAsync(username);
            if (user == null || !user.IsActive) return false;

            user.FullName = request.FullName?.Trim();
            user.Phone = request.Phone?.Trim();
            user.Address = request.Address?.Trim();

            return await _userRepo.UpdateAsync(user);
        }



        public async Task<bool> ResendOtpAsync(string email, string type)
        {
            var emailLower = email.Trim().ToLower();
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Check Rate Limit
            var limitKey = type == "register" ? $"otp_limit_{emailLower}" : $"reset_limit_{emailLower}";
            if (_cache.TryGetValue(limitKey, out int attempts))
            {
                if (attempts >= 5) return false;
                _cache.Set(limitKey, attempts + 1, TimeSpan.FromHours(2));
            }
            else
            {
                _cache.Set(limitKey, 1, TimeSpan.FromHours(2));
            }

            if (type == "register")
            {
                var cacheKey = $"pending_reg_{emailLower}";
                if (_cache.TryGetValue(cacheKey, out Tuple<RegisterRequestDTO, string, UserRole>? cachedData) && cachedData != null)
                {
                    var newData = new Tuple<RegisterRequestDTO, string, UserRole>(cachedData.Item1, otpCode, cachedData.Item3);
                    _cache.Set(cacheKey, newData, TimeSpan.FromMinutes(2));
                    await _emailService.SendEmailAsync(emailLower, "Xác nhận đăng ký tài khoản (Gửi lại)", $"Mã xác nhận (OTP) mới của bạn là: {otpCode}\nMã có hiệu lực trong 2 phút.");
                    return true;
                }
            }
            else if (type == "forgot")
            {
                var user = await _userRepo.GetUserByEmailAsync(emailLower);
                if (user != null && user.IsActive)
                {
                    _cache.Set($"reset_pwd_{emailLower}", otpCode, TimeSpan.FromMinutes(2));
                    await _emailService.SendEmailAsync(emailLower, "Khôi phục mật khẩu (Gửi lại)", $"Mã xác nhận (OTP) mới của bạn là: {otpCode}\nMã có hiệu lực trong 2 phút.");
                    return true;
                }
            }

            return false;
        }

        public async Task<AuthResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

            var user = await _userRepo.GetUserByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new UserAccount
                {
                    FullName = payload.Name ?? string.Empty,
                    Username = payload.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    Email = payload.Email,
                    Role = UserRole.Customer,
                    IsActive = true
                };

                await _userRepo.AddAsync(user);
            }
            else if (!user.IsActive)
            {
                throw new InvalidOperationException("Tài khoản người dùng đã bị khóa.");
            }

            return GenerateToken(user);
        }

        private AuthResponseDTO GenerateToken(UserAccount user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"] ?? throw new InvalidOperationException("Thiếu Jwt:Key");
            var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Thiếu Jwt:Issuer");
            var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Thiếu Jwt:Audience");
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
