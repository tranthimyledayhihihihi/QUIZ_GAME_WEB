// Models/Implementations/AuthService.cs
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels; // SỬ DỤNG LOGINRESPONSEMODEL VÀ VIEWMODELS
using QUIZ_GAME_WEB.Models.CoreEntities;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<LoginResponseModel?> DangNhapAsync(DangNhapModel model)
        {
            // 1. Truy cập DB để lấy User
            var user = await _unitOfWork.Users.GetByTenDangNhapAsync(model.TenDangNhap);

            // Kiểm tra User tồn tại, trạng thái và xác thực mật khẩu
            if (user == null || !user.TrangThai || !VerifyPassword(model.MatKhau, user.MatKhau))
            {
                return null;
            }

            // 2. Lấy Role và Tạo Token
            var role = await _unitOfWork.Users.GetRoleByUserIdAsync(user.UserID);
            string userRole = role?.TenVaiTro ?? "Player";

            string token = GenerateJwtToken(user, userRole);

            // 3. Cập nhật trạng thái trong DB
            user.LanDangNhapCuoi = DateTime.Now;
            // (Thêm logic lưu PhienDangNhap vào DB nếu cần)
            await _unitOfWork.CompleteAsync();

            // 4. Trả về Response DTO
            return new LoginResponseModel
            {
                Token = token,
                HoTen = user.HoTen,
                VaiTro = userRole
            };
        }

        public async Task<bool> DangKyAsync(DangKyModel model)
        {
            // 1. Kiểm tra tính duy nhất (sử dụng Repository)
            if (await _unitOfWork.Users.IsEmailInUseAsync(model.Email) ||
                await _unitOfWork.Users.GetByTenDangNhapAsync(model.TenDangNhap) != null)
            {
                return false;
            }

            // 2. Tạo User mới và Hash mật khẩu
            var newUser = new NguoiDung
            {
                TenDangNhap = model.TenDangNhap,
                MatKhau = HashPassword(model.MatKhau), // Hash Mật khẩu
                Email = model.Email,
                HoTen = model.HoTen,
                NgayDangKy = DateTime.Now,
                TrangThai = true
            };

            _unitOfWork.Users.Add(newUser);
            await _unitOfWork.CompleteAsync();

            // Tùy chọn: Gán Vai trò 'Player' mặc định nếu cần

            return true;
        }

        public Task<bool> DangXuatAsync(int userId)
        {
            // Logic nghiệp vụ: Vô hiệu hóa PhienDangNhap (tìm và cập nhật TrangThai = false)
            // (Cần bổ sung logic này vào Repository)
            // _unitOfWork.CompleteAsync(); 
            return Task.FromResult(true);
        }

        public Task<string> GenerateJwtTokenAsync(int userId)
        {
            // Logic nghiệp vụ: Tạo một JWT token mới (ví dụ: cho refresh token)
            // ...
            return Task.FromResult("NEW_REFRESHED_TOKEN_GENERATED");
        }

        // ===============================================
        // 🛠️ HÀM HỖ TRỢ BẢO MẬT (PRIVATE HELPERS)
        // ===============================================

        private string HashPassword(string password)
        {
            // THỰC TẾ: Sử dụng BCrypt.Net hoặc SHA-256/Argon2 + muối (salt)
            return $"HASHED_PASSWORD_{password}_{_config["Salt"]}"; // Giả lập Hash
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            // THỰC TẾ: So sánh input hash với stored hash
            return HashPassword(inputPassword) == storedHash;
        }

        private string GenerateJwtToken(NguoiDung user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim(ClaimTypes.Role, role)
            };

            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}