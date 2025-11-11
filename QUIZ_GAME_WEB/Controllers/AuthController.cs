using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QUIZ_GAME.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (await _db.NguoiDung.AnyAsync(x => x.TenDangNhap == request.TenDangNhap))
                    return BadRequest("Tên đăng nhập đã tồn tại");

                var newUser = new NguoiDung
                {
                    TenDangNhap = request.TenDangNhap,
                    MatKhau = request.MatKhau,
                    Email = request.Email,
                    HoTen = request.HoTen,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                };

                _db.NguoiDung.Add(newUser);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Đăng ký thành công", userId = newUser.UserID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        /// <summary>
        /// Đăng nhập và nhận JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _db.NguoiDung
                    .Where(x => x.TenDangNhap == request.Username)
                    .Select(x => new {
                        x.UserID,
                        x.TenDangNhap,
                        x.MatKhau,
                        x.Email,
                        x.HoTen,
                        x.TrangThai
                    })
                    .FirstOrDefaultAsync();

                if (user == null || user.MatKhau != request.Password)
                    return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

                if (!user.TrangThai)
                    return Unauthorized("Tài khoản đã bị khóa");

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "SECRET_KEY");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                        new Claim(ClaimTypes.Name, user.TenDangNhap ?? "")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    token = tokenString,
                    user = new
                    {
                        user.UserID,
                        user.TenDangNhap,
                        user.HoTen,
                        user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var user = await _db.NguoiDung
                    .FirstOrDefaultAsync(x => x.TenDangNhap == request.Username);

                if (user == null || user.MatKhau != request.OldPassword)
                    return Unauthorized("Thông tin không chính xác");

                user.MatKhau = request.NewPassword;
                await _db.SaveChangesAsync();

                return Ok("Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        /// <summary>
        /// Xác thực token
        /// </summary>
        [HttpGet("verify")]
        public IActionResult VerifyToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userId)
                ? Unauthorized("Token không hợp lệ")
                : Ok(new { userId, message = "Token hợp lệ" });
        }
    }

    // MODELS
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        public string MatKhau { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? HoTen { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}