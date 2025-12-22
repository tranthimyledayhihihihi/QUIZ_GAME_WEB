using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly QuizGameContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(QuizGameContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Hàm hỗ trợ lấy UserID từ JWT
    private int GetUserIdFromClaim()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int userId))
        {
            throw new UnauthorizedAccessException("User ID không hợp lệ trong token.");
        }
        return userId;
    }

    // ===============================================
    // 🔑 API 1: ĐĂNG NHẬP (LOGIN)
    // ===============================================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DangNhapModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // ✅ CẢI TIẾN: Include VaiTro để GetUserRoleFromDatabase không cần truy vấn lại
        var user = await _context.NguoiDungs
                                 .Include(u => u.VaiTro)
                                 .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap);

        if (user == null || !user.TrangThai)
        {
            return Unauthorized(new { message = "Thông tin đăng nhập không hợp lệ hoặc tài khoản bị khóa." });
        }

        if (!VerifyPassword(model.MatKhau, user.MatKhau))
        {
            return Unauthorized(new { message = "Mật khẩu không đúng." });
        }

        // Lấy vai trò trực tiếp từ Navigation Property
        string userRole = user.VaiTro?.TenVaiTro ?? "Player";
        string token = GenerateJwtToken(user, userRole);

        // ✅ BỔ SUNG: LƯU PHIÊN ĐĂNG NHẬP MỚI
        var newSession = new PhienDangNhap
        {
            UserID = user.UserID,
            ThoiGianBatDau = DateTime.Now,
            Token = token,
            ThoiGianKetThuc = null,
            TrangThai = true
        };
        await _context.PhienDangNhaps.AddAsync(newSession);

        user.LanDangNhapCuoi = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok(new LoginResponseModel
        {
            Token = token,
            HoTen = user.HoTen,
            VaiTro = userRole
        });
    }

    // ===============================================
    // 🔑 API 2: ĐĂNG KÝ (REGISTER)
    // ===============================================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DangKyModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _context.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap || u.Email == model.Email))
        {
            return Conflict(new { message = "Tên đăng nhập hoặc Email đã được sử dụng." });
        }

        // 1. TÌM VaiTroID (Giả định ID 3 là Player)
        var playerRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Player");

        // CẦN XỬ LÝ LỖI: Nếu bảng VaiTro trống (chưa chạy Seed Data)
        if (playerRole == null)
        {
            return StatusCode(500, new { message = "Lỗi hệ thống: Không tìm thấy vai trò 'Player'. Vui lòng kiểm tra Seed Data." });
        }

        // 2. TẠO User mới và GÁN VaiTroID
        var newUser = new NguoiDung
        {
            TenDangNhap = model.TenDangNhap,
            MatKhau = HashPassword(model.MatKhau),
            Email = model.Email,
            HoTen = model.HoTen,
            NgayDangKy = DateTime.Now,
            TrangThai = true,
            // ✅ KHẮC PHỤC LỖI: GÁN VaiTroID BẮT BUỘC
            VaiTroID = playerRole.VaiTroID
        };

        // 3. TẠO CÀI ĐẶT mặc định (CaiDatNguoiDung)
        var defaultSettings = new CaiDatNguoiDung
        {
            // Không cần gán UserID ở đây, EF Core sẽ tự gán sau khi lưu
            AmThanh = true,
            NhacNen = true,
            ThongBao = true,
            NgonNgu = "vi",
            // Quan hệ 1:1 sẽ được thiết lập tự động
        };
        // Gán Navigation Property để thiết lập quan hệ 1:1
        newUser.CaiDat = defaultSettings;


        await _context.NguoiDungs.AddAsync(newUser);

        // 4. LƯU THAY ĐỔI
        // Hàm SaveChangesAsync sẽ thực hiện lệnh INSERT cho NguoiDung và CaiDatNguoiDung
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log ex.InnerException.Message để xem lỗi SQL chính xác
            return StatusCode(500, new { message = "Lỗi khi lưu dữ liệu, có thể do cấu hình database: " + ex.InnerException?.Message });
        }


        return StatusCode(201, new { message = "Đăng ký thành công." });
    }

    // ===============================================
    // 🔑 API 3: ĐỔI MẬT KHẨU (CHANGE PASSWORD)
    // ... (Giữ nguyên)
    // ===============================================
    [HttpPost("change-password")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        int userId;
        try
        {
            userId = GetUserIdFromClaim();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Token không hợp lệ.");
        }

        var user = await _context.NguoiDungs.FindAsync(userId);
        if (user == null) return NotFound("Người dùng không tồn tại.");

        if (!VerifyPassword(model.CurrentPassword, user.MatKhau))
        {
            return Unauthorized("Mật khẩu hiện tại không đúng.");
        }

        user.MatKhau = HashPassword(model.NewPassword);
        _context.NguoiDungs.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Đổi mật khẩu thành công." });
    }

    // ===============================================
    // 🔑 API 4: ĐĂNG XUẤT (LOGOUT)
    // ... (Giữ nguyên)
    // ===============================================
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Logout()
    {
        int userId;
        try
        {
            userId = GetUserIdFromClaim();
        }
        catch
        {
            // Không lấy được user → coi như logout OK
            return Ok(new { message = "Đăng xuất thành công." });
        }

        // Lấy token hiện tại
        string token = HttpContext.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "")
            .Trim();

        // 🔍 Tìm PHIÊN ĐANG HOẠT ĐỘNG (CHUẨN)
        var activeSession = await _context.PhienDangNhaps
            .Where(s =>
                s.UserID == userId &&
                s.Token == token &&
                s.ThoiGianKetThuc == null) // ✅ CHUẨN
            .OrderByDescending(s => s.ThoiGianBatDau)
            .FirstOrDefaultAsync();

        if (activeSession != null)
        {
            // ✅ KẾT THÚC PHIÊN
            activeSession.ThoiGianKetThuc = DateTime.UtcNow;

            // ⚠️ Nếu bạn vẫn giữ TrangThai → sync cho đúng
            activeSession.TrangThai = false;

            _context.PhienDangNhaps.Update(activeSession);
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Đăng xuất thành công." });
    }


    // ===============================================
    // 🛠️ HÀM HỖ TRỢ (SECURITY & DATA ACCESS)
    // ===============================================

    private string HashPassword(string password)
    {
        return $"hashed_{password}_password";
    }

    private bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        return hashedPassword == HashPassword(inputPassword);
    }

    // ✅ CẢI TIẾN LOGIC: Lấy vai trò Admin từ bảng Admin
    private async Task<string> GetUserRoleFromDatabase(int userId)
    {
        // Truy vấn bảng Admin để xem người dùng này có vai trò Admin/Moderator không
        var role = await (from a in _context.Admins
                          join r in _context.VaiTros on a.VaiTroID equals r.VaiTroID
                          where a.UserID == userId
                          select r.TenVaiTro)
                          .FirstOrDefaultAsync();

        // Nếu không phải Admin/Moderator, mặc định là Player.
        return role ?? "Player";
    }

    private string GenerateJwtToken(NguoiDung user, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.TenDangNhap),
            new Claim(ClaimTypes.Role, role)
        };
        if (role == "SuperAdmin")
        {
            claims.Add(new Claim(ClaimTypes.Role, "SuperAdmin"));
        }
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}