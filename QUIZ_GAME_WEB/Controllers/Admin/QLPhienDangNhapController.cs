using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;
using System.Linq;

[Route("api/admin/phiendangnhap")]
[ApiController]
// Chỉ SuperAdmin và Moderator mới có quyền quản lý phiên
[Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
public class QLPhienDangNhapController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QLPhienDangNhapController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // ===============================================
    // 1. LẤY DANH SÁCH TẤT CẢ PHIÊN (PHÂN TRANG & LỌC)
    // ===============================================
    [HttpGet]
    public async Task<IActionResult> GetAllSessions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] bool? isActive = null)
    {
        // Giả định LoginSessions.GetQueryable() trả về IQueryable<PhienDangNhap>
        var query = _unitOfWork.LoginSessions.GetQueryable();

        // Eager Load thông tin người dùng
        query = query.Include(s => s.NguoiDung);

        // Lọc theo trạng thái hoạt động (IsActive)
        if (isActive.HasValue)
        {
            // Giả định trạng thái được tính dựa trên ThoiGianKetThuc (null = đang hoạt động)
            if (isActive.Value)
            {
                query = query.Where(s => s.ThoiGianKetThuc == null);
            }
            else
            {
                query = query.Where(s => s.ThoiGianKetThuc != null);
            }
        }

        // Lọc theo từ khóa (Username)
        if (!string.IsNullOrEmpty(keyword))
        {
            string lowerKeyword = keyword.ToLower();
            query = query.Where(s => s.NguoiDung != null && s.NguoiDung.TenDangNhap.ToLower().Contains(lowerKeyword));
        }

        var today = DateTime.Today;

        // Tổng số phiên (đã có)
        var totalCount = await query.CountAsync();

        // ✅ SỐ PHIÊN HÔM NAY – TOÀN BẢNG
        var todayCount = await _unitOfWork.LoginSessions
            .GetQueryable()
            .CountAsync(s => s.ThoiGianBatDau >= today
                          && s.ThoiGianBatDau < today.AddDays(1));

        var pagedSessions = await query
            .OrderByDescending(s => s.ThoiGianBatDau)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            // Ánh xạ sang Anonymous Object để làm sạch dữ liệu
            .Select(s => new
            {
                s.SessionID,
                s.UserID,
                Username = s.NguoiDung.TenDangNhap,
                s.ThoiGianBatDau,
                s.ThoiGianKetThuc,
                IsActive = s.ThoiGianKetThuc == null,
                s.IPAddress,
                s.DeviceType
            })
            .ToListAsync();

        return Ok(new
        {
            Sessions = pagedSessions,
            TotalCount = totalCount,

            // ✅ THÊM DÒNG NÀY
            TodayCount = todayCount
        });
    }

    // ===============================================
    // 2. LẤY CHI TIẾT PHIÊN ĐĂNG NHẬP
    // ===============================================
    [HttpGet("{sessionId:int}")]
    public async Task<IActionResult> GetSessionDetails(int sessionId)
    {
        var session = await _unitOfWork.LoginSessions.GetByIdAsync(sessionId);

        if (session == null)
            return NotFound(new { message = $"Không tìm thấy Phiên đăng nhập ID: {sessionId}." });

        // Kiểm tra Session có thuộc tính điều hướng NguoiDung không, nếu không, phải Include

        return Ok(new
        {
            session.SessionID,
            session.UserID,
            Username = session.NguoiDung?.TenDangNhap,
            session.ThoiGianBatDau,
            session.ThoiGianKetThuc,
            IsActive = session.ThoiGianKetThuc == null,
            session.IPAddress,
            session.DeviceType
        });
    }

    // ===============================================
    // 3. BUỘC ĐĂNG XUẤT (Force Logout)
    // ===============================================
    [HttpPost("buoc-dang-xuat/{sessionId:int}")]
    [Authorize(Roles = "SuperAdmin")] // Chỉ SuperAdmin mới có quyền can thiệp phiên
    public async Task<IActionResult> ForceLogout(int sessionId)
    {
        var session = await _unitOfWork.LoginSessions.GetByIdAsync(sessionId);

        if (session == null)
            return NotFound(new { message = $"Không tìm thấy Phiên đăng nhập ID: {sessionId}." });

        if (session.ThoiGianKetThuc != null)
            return BadRequest(new { message = "Phiên này đã kết thúc (không hoạt động)." });

        // Cập nhật ThoiGianKetThuc để vô hiệu hóa phiên
        session.ThoiGianKetThuc = DateTime.UtcNow;
        _unitOfWork.LoginSessions.Update(session);
        await _unitOfWork.CompleteAsync();

        // Gửi tín hiệu thông báo người dùng bị buộc đăng xuất (qua SignalR hoặc hệ thống khác)

        return Ok(new { message = $"Đã buộc đăng xuất thành công User ID {session.UserID}." });
    }
}