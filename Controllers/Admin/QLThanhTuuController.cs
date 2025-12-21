using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels; // Chứa ThanhTuu và ThanhTuuDefinition
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

[Route("api/admin/thanhtuu")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
public class QLThanhTuuController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QLThanhTuuController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Hàm phụ trợ để truy cập Repository ThanhTuuDefinition
    private IGenericRepository<ThanhTuuDefinition> GetAchievementDefinitionRepository()
    {
        // ✅ FIX: Sử dụng tên Repository mới: AchievementDefinitions
        return _unitOfWork.AchievementDefinitions;
    }

    // Hàm phụ trợ để lấy AdminID hiện tại
    private int GetCurrentAdminId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int adminId))
        {
            // Tạm thời trả về ID của người dùng đăng nhập
            return adminId;
        }
        return 0;
    }

    // ===============================================
    // 1. LẤY DANH SÁCH TẤT CẢ ĐỊNH NGHĨA
    // ===============================================
    [HttpGet]
    public async Task<IActionResult> GetAllAchievements()
    {
        var repo = GetAchievementDefinitionRepository();
        var achievements = await repo.GetQueryable().Include(d => d.Admin).ToListAsync();
        return Ok(achievements);
    }

    // ===============================================
    // 2. LẤY CHI TIẾT ĐỊNH NGHĨA
    // ===============================================
    [HttpGet("{thanhTuuId:int}")]
    public async Task<IActionResult> GetAchievementById(int thanhTuuId)
    {
        var repo = GetAchievementDefinitionRepository();
        var achievement = await repo.GetQueryable()
                                    .Include(d => d.Admin)
                                    .FirstOrDefaultAsync(d => d.DefinitionID == thanhTuuId);

        if (achievement == null)
            return NotFound(new { message = $"Không tìm thấy Định nghĩa Thành tựu với ID: {thanhTuuId}." });

        return Ok(achievement);
    }

    // ===============================================
    // 3. THÊM ĐỊNH NGHĨA MỚI (Dùng Input Model)
    // ===============================================
    [HttpPost]
    [Authorize(Roles = "SuperAdmin, Moderator")]
    public async Task<IActionResult> CreateAchievement([FromBody] ThanhTuuCreateUpdateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var repo = GetAchievementDefinitionRepository();
        var adminId = GetCurrentAdminId();

        var newDefinition = new ThanhTuuDefinition
        {
            TenThanhTuu = model.TenThanhTuu,
            MoTa = model.MoTa,
            DiemThuong = model.DiemThuong,
            LoaiDieuKien = model.LoaiThanhTuu,
            GiaTriCanThiet = model.GiaTriCanThiet,
            AchievementCode = model.AchievementCode,
            BieuTuong = model.BieuTuong,
            AdminID = adminId
        };

        // TODO: Kiểm tra trùng AchievementCode

        repo.Add(newDefinition);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetAchievementById), new { thanhTuuId = newDefinition.DefinitionID }, newDefinition);
    }

    // ===============================================
    // 4. CẬP NHẬT ĐỊNH NGHĨA (Dùng Input Model)
    // ===============================================
    [HttpPut("{thanhTuuId:int}")]
    public async Task<IActionResult> UpdateAchievement(int thanhTuuId, [FromBody] ThanhTuuCreateUpdateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var repo = GetAchievementDefinitionRepository();
        var existingDefinition = await repo.GetByIdAsync(thanhTuuId);

        if (existingDefinition == null)
            return NotFound(new { message = $"Không tìm thấy Định nghĩa Thành tựu với ID: {thanhTuuId}." });

        existingDefinition.TenThanhTuu = model.TenThanhTuu;
        existingDefinition.MoTa = model.MoTa;
        existingDefinition.DiemThuong = model.DiemThuong;
        existingDefinition.LoaiDieuKien = model.LoaiThanhTuu;
        existingDefinition.GiaTriCanThiet = model.GiaTriCanThiet;
        existingDefinition.AchievementCode = model.AchievementCode;
        existingDefinition.BieuTuong = model.BieuTuong;

        repo.Update(existingDefinition);
        await _unitOfWork.CompleteAsync();

        return Ok(new
        {
            message = $"Cập nhật Định nghĩa Thành tựu '{existingDefinition.TenThanhTuu}' thành công!",
            data = existingDefinition
        });
    }

    // ===============================================
    // 5. XÓA ĐỊNH NGHĨA
    // ===============================================
    [HttpDelete("{thanhTuuId:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteAchievement(int thanhTuuId)
    {
        var repo = GetAchievementDefinitionRepository();
        var definition = await repo.GetByIdAsync(thanhTuuId);

        if (definition == null)
            return NotFound(new { message = $"Không tìm thấy Định nghĩa Thành tựu với ID: {thanhTuuId}." });

        // ✅ FIX: Sử dụng tên Repository mới: UserAchievements
        var hasUserAchievements = await _unitOfWork.UserAchievements.GetQueryable().AnyAsync(u => u.DefinitionID == thanhTuuId);

        if (hasUserAchievements)
        {
            return BadRequest(new { message = "Không thể xóa định nghĩa này vì đã có người dùng đạt được thành tựu này." });
        }

        repo.Delete(definition);
        await _unitOfWork.CompleteAsync();

        return Ok(new { message = $"Đã xóa Định nghĩa Thành tựu ID: {thanhTuuId}." });
    }
}