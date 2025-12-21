using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/admin/nguoidung")]
[ApiController]
// Chỉ cho phép Admin/Moderator truy cập chức năng quản lý người dùng
[Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
public class QLNguoiDungController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QLNguoiDungController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Hàm phụ trợ: Lấy ID của Admin/Moderator đang đăng nhập
    private int LayIdAdminHienTai()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        // Kiểm tra null và Parse an toàn để tránh lỗi Null Reference
        if (idClaim != null && int.TryParse(idClaim.Value, out int userId)) return userId;
        throw new UnauthorizedAccessException("ID người dùng không hợp lệ trong token.");
    }

    // ===============================================
    // 1. LẤY DANH SÁCH TÀI KHOẢN (PHÂN TRANG & LỌC)
    // ===============================================
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? keyword = null)
    {
        // Lấy tất cả người dùng (sử dụng GetQueryable để hỗ trợ LINQ)
        var query = _unitOfWork.Users.GetQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            string lowerKeyword = keyword.ToLower();
            query = query.Where(u =>
                (u.TenDangNhap != null && u.TenDangNhap.ToLower().Contains(lowerKeyword)) ||
                (u.Email != null && u.Email.ToLower().Contains(lowerKeyword)));
        }

        var pagedUsers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            // ✅ Projection: Ánh xạ để làm sạch dữ liệu và tránh JSON cycle
            .Select(u => new
            {
                u.UserID,
                u.TenDangNhap,
                u.Email,
                u.HoTen,
                u.NgayDangKy,
                u.TrangThai,
                u.VaiTroID,
                // Lấy tên Role
                RoleName = u.VaiTro.TenVaiTro
            })
            .ToListAsync();

        return Ok(pagedUsers);
    }

    // ===============================================
    // 2. LẤY CHI TIẾT TÀI KHOẢN
    // ===============================================
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        // Giả định GetByIdAsync sẽ bao gồm (Include) thông tin VaiTro
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        // ✅ Sửa lỗi trả về: Dùng NotFound(object)
        if (user == null) return NotFound(new { message = "Không tìm thấy người dùng." });

        // Trả về DTO/ViewModel (Đảm bảo không trả về MatKhau)
        return Ok(new
        {
            user.UserID,
            user.TenDangNhap,
            user.Email,
            user.HoTen,
            user.NgayDangKy,
            user.TrangThai,
            user.VaiTroID,
            RoleName = user.VaiTro.TenVaiTro
        });
    }

    // ===============================================
    // 3. KHÓA TÀI KHOẢN (Ban User)
    // ===============================================
    [HttpPost("khoa/{userId:int}")]
    [Authorize(Roles = "SuperAdmin, Moderator")]
    public async Task<IActionResult> LockUser(int userId)
    {
        var userToLock = await _unitOfWork.Users.GetByIdAsync(userId);

        if (userToLock == null) return NotFound(new { message = "Không tìm thấy người dùng." });

        // Quy tắc: Không cho phép khóa tài khoản Admin/Moderator khác (VaiTroID <= 2)
        if (userToLock.VaiTroID <= 2)
        {
            // ✅ Sửa lỗi trả về: Dùng StatusCode 403 Forbidden
            return StatusCode(403, new { message = "Không thể khóa tài khoản quản trị cấp cao." });
        }

        if (userToLock.TrangThai == false)
            return BadRequest(new { message = "Tài khoản này đã bị khóa." });

        userToLock.TrangThai = false; // false = Bị khóa/Ban
        _unitOfWork.Users.Update(userToLock);
        await _unitOfWork.CompleteAsync();

        return Ok(new { message = $"Đã khóa tài khoản {userToLock.TenDangNhap}." });
    }

    // ===============================================
    // 4. MỞ KHÓA TÀI KHOẢN (Unban User)
    // ===============================================
    [HttpPost("mo-khoa/{userId:int}")]
    [Authorize(Roles = "SuperAdmin, Moderator")]
    public async Task<IActionResult> UnlockUser(int userId)
    {
        var userToUnlock = await _unitOfWork.Users.GetByIdAsync(userId);

        if (userToUnlock == null) return NotFound(new { message = "Không tìm thấy người dùng." });

        if (userToUnlock.TrangThai == true)
            return BadRequest(new { message = "Tài khoản này đang hoạt động." });

        userToUnlock.TrangThai = true; // true = Đang hoạt động
        _unitOfWork.Users.Update(userToUnlock);
        await _unitOfWork.CompleteAsync();

        return Ok(new { message = $"Đã mở khóa tài khoản {userToUnlock.TenDangNhap}." });
    }

    // ===============================================
    // 5. CẬP NHẬT VAI TRÒ (Phân quyền)
    // ===============================================
    [HttpPost("phan-quyen/{userId:int}/{newRoleId:int}")]
    [Authorize(Roles = "SuperAdmin")] // CHỈ SUPERADMIN MỚI ĐƯỢC PHÂN QUYỀN
    public async Task<IActionResult> UpdateUserRole(int userId, int newRoleId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return NotFound(new { message = "Không tìm thấy người dùng." });

        // 1. Kiểm tra Vai trò mới có tồn tại không
        var role = await _unitOfWork.Roles.GetByIdAsync(newRoleId);
        if (role == null) return BadRequest(new { message = "ID Vai trò không hợp lệ." });

        // 2. Quy tắc: Không cho phép hạ cấp SuperAdmin (VaiTroID = 1)
        if (user.VaiTroID == 1 && newRoleId != 1)
        {
            return StatusCode(403, new { message = "Không thể thay đổi vai trò của SuperAdmin." });
        }

        user.VaiTroID = newRoleId;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.CompleteAsync();

        return Ok(new { message = $"Đã cập nhật vai trò của {user.TenDangNhap} thành {role.TenVaiTro}." });
    }
}