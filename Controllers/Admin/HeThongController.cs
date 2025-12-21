using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels; // TroGiup
using QUIZ_GAME_WEB.Models.CoreEntities; // SystemSettings

namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Route("api/admin/he-thong")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
    public class HeThongController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public HeThongController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===============================================
        // Hàm phụ trợ để lấy Repository (Giống QLDoKhoController)
        // ===============================================

        private IGenericRepository<TroGiup> GetHelperRepository()
        {
            // Giả định IQuizRepository có hàm trả về Repo cho TroGiup
            return _unitOfWork.Quiz.GetHelperRepository();
        }

        private IGenericRepository<SystemSetting> GetSettingsRepository()
        {
            // Giả định IQuizRepository hoặc IUnitOfWork có cách lấy Repo cho Settings
            return _unitOfWork.Quiz.GetSettingsRepository();
        }

        // ===============================================
        // 1. QUẢN LÝ TRỢ GIÚP (TroGiup)
        // ===============================================

        [HttpGet("tro-giup")]
        public async Task<IActionResult> GetTroGiups()
        {
            var repo = GetHelperRepository();
            var list = await repo.GetAllAsync();
            return Ok(list);
        }
        [HttpPost("tro-giup")]
        public async Task<IActionResult> PostTroGiup([FromBody] TroGiup troGiup)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Đảm bảo ID bằng 0 để SQL Server tự động sinh ID mới
            troGiup.TroGiupID = 0;

            var repo = GetHelperRepository();
            repo.Add(troGiup);

            // Lưu xuống database
            await _unitOfWork.CompleteAsync();

            // Trả về kết quả kèm ID đã được sinh tự động
            return CreatedAtAction(nameof(GetTroGiups), new { id = troGiup.TroGiupID }, troGiup);
        }

        [HttpDelete("tro-giup/{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteTroGiup(int id)
        {
            var repo = GetHelperRepository();
            var troGiup = await repo.GetByIdAsync(id);
            if (troGiup == null) return NotFound();

            repo.Delete(troGiup);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // ===============================================
        // 2. QUẢN LÝ CẤU HÌNH HỆ THỐNG (SystemSettings)
        // ===============================================

        [HttpGet("settings")]
        public async Task<IActionResult> GetAllSettings()
        {
            var repo = GetSettingsRepository();
            var settings = await repo.GetAllAsync();
            return Ok(settings);
        }
        [HttpPost("settings")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> PostSetting([FromBody] SystemSetting newSetting)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var repo = _unitOfWork.Quiz.GetSettingsRepository();

            // Kiểm tra xem Key đã tồn tại chưa
            var exists = await repo.GetQueryable().AnyAsync(s => s.Key == newSetting.Key);
            if (exists) return Conflict(new { message = $"Cấu hình '{newSetting.Key}' đã tồn tại." });

            repo.Add(newSetting);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetAllSettings), new { key = newSetting.Key }, newSetting);
        }

        [HttpPut("settings/{key}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateSetting(string key, [FromBody] SystemSetting updatedSetting)
        {
            if (key != updatedSetting.Key)
                return BadRequest(new { message = "Khóa (Key) không khớp." });

            var repo = _unitOfWork.Quiz.GetSettingsRepository();

            // SỬA LỖI: Dùng await và FirstOrDefaultAsync để tìm theo string Key thay vì GetByIdAsync(int)
            var setting = await repo.GetQueryable()
                                    .FirstOrDefaultAsync(s => s.Key == key);

            if (setting == null)
                return NotFound(new { message = $"Không tìm thấy cấu hình: {key}" });

            // Cập nhật giá trị
            setting.Value = updatedSetting.Value;
            setting.MoTa = updatedSetting.MoTa;

            repo.Update(setting);

            // Đảm bảo có await tại đây để giải quyết cảnh báo CS1998
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = $"Cập nhật cấu hình '{key}' thành công." });
        }
    }
}