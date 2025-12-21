using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProfileController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly IProfileService _profileService;

        public ProfileController(QuizGameContext context, IProfileService profileService)
        {
            _context = context;
            _profileService = profileService;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        // ===============================================
        // 1. LẤY THÔNG TIN HỒ SƠ (GET: api/user/profile/me)
        // ===============================================
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token." });

                // ✅ SỬA LỖI: THÊM .Include(u => u.VaiTro) để tải thông tin phân quyền
                var user = await _context.NguoiDungs
                    .Include(u => u.CaiDat)
                    .Include(u => u.VaiTro) // 🔑 Khắc phục lỗi VaiTro là null
                    .FirstOrDefaultAsync(u => u.UserID == userId.Value);

                if (user == null)
                    return NotFound(new { message = "Người dùng không tồn tại." });

                // Ánh xạ sang Anonymous Type (bao gồm cả Vai trò)
                var result = new
                {
                    user.UserID,
                    user.TenDangNhap,
                    user.Email,
                    user.HoTen,
                    user.AnhDaiDien,
                    user.NgayDangKy,
                    user.LanDangNhapCuoi,
                    VaiTro = user.VaiTro?.TenVaiTro, // ✅ Bây giờ VaiTro sẽ không null
                    CaiDat = user.CaiDat == null ? null : new
                    {
                        user.CaiDat.AmThanh,
                        user.CaiDat.NhacNen,
                        user.CaiDat.ThongBao,
                        user.CaiDat.NgonNgu
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi truy vấn hồ sơ: " + ex.Message });
            }
        }

        // ===============================================
        // 2. CẬP NHẬT HỒ SƠ (PUT: api/user/profile/me)
        // ===============================================
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfileUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token." });

                var success = await _profileService.UpdateProfileAsync(userId.Value, model);

                if (!success)
                    return BadRequest(new { message = "Cập nhật thất bại. Vui lòng kiểm tra email hoặc tên người dùng." });

                return Ok(new { message = "Cập nhật hồ sơ thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật hồ sơ: " + ex.Message });
            }
        }

        // [Tái tạo Model lồng SettingUpdateModel]
        public class SettingUpdateModel
        {
            public bool AmThanh { get; set; } = true;
            public bool NhacNen { get; set; } = true;
            public bool ThongBao { get; set; } = true;
            [Required]
            public string NgonNgu { get; set; } = "vi";
        }

        // ===============================================
        // 3. CẬP NHẬT CÀI ĐẶT (PUT: api/user/profile/settings)
        // ===============================================
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] SettingUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token." });

                var success = await _profileService.UpdateUserSettingAsync(
                    userId.Value,
                    model.AmThanh,
                    model.NhacNen,
                    model.ThongBao,
                    model.NgonNgu
                );

                if (!success)
                    return NotFound(new { message = "Người dùng không tồn tại hoặc cập nhật cài đặt thất bại." });

                return Ok(new { message = "Cập nhật cài đặt thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật cài đặt: " + ex.Message });
            }
        }
    }
}