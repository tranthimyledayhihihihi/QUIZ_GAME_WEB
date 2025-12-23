using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Route("api/admin/vaitro")]
    [ApiController]
    // Chỉ SuperAdmin mới có quyền quản lý cấu trúc vai trò và quyền hạn
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin")]
    public class QLVaiTroController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QLVaiTroController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===============================================
        // 1. LẤY DANH SÁCH TẤT CẢ VAI TRÒ
        // ===============================================
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _unitOfWork.Roles.GetQueryable()
                .Select(r => new {
                    r.VaiTroID,
                    r.TenVaiTro,
                    SoLuongNguoiDung = r.NguoiDungs.Count()
                })
                .ToListAsync();

            return Ok(roles);
        }

        // ===============================================
        // 2. LẤY CHI TIẾT VAI TRÒ VÀ CÁC QUYỀN ĐI KÈM
        // ===============================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoleDetail(int id)
        {
            var role = await _unitOfWork.Roles.GetQueryable()
                .Where(r => r.VaiTroID == id)
                .Select(r => new {
                    r.VaiTroID,
                    r.TenVaiTro,
                    QuyenHienTai = r.VaiTro_Quyens.Select(vq => new {
                        vq.Quyen.QuyenID,
                        vq.Quyen.TenQuyen,
                        vq.Quyen.MoTa
                    })
                })
                .FirstOrDefaultAsync();

            if (role == null) return NotFound(new { message = "Không tìm thấy vai trò." });

            return Ok(role);
        }

        // ===============================================
        // 3. THÊM VAI TRÒ MỚI
        // ===============================================
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string tenVaiTro)
        {
            if (string.IsNullOrWhiteSpace(tenVaiTro))
                return BadRequest(new { message = "Tên vai trò không được để trống." });

            var exists = await _unitOfWork.Roles.GetQueryable().AnyAsync(r => r.TenVaiTro == tenVaiTro);
            if (exists) return Conflict(new { message = "Vai trò này đã tồn tại." });

            var newRole = new VaiTro { TenVaiTro = tenVaiTro };
            _unitOfWork.Roles.Add(newRole);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Tạo vai trò thành công.", roleId = newRole.VaiTroID });
        }

        // ===============================================
        // 4. GÁN QUYỀN CHO VAI TRÒ (Update VaiTro_Quyen)
        // ===============================================
        [HttpPost("{roleId:int}/gan-quyen")]
        public async Task<IActionResult> AssignPermissions(int roleId, [FromBody] List<int> quyenIds)
        {
            // Kiểm tra vai trò tồn tại
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null) return NotFound(new { message = "Không tìm thấy vai trò." });

            // Sử dụng hàm Sync đã viết trong PermissionRepository (nếu bạn đã đăng ký nó trong UoW)
            var success = await _unitOfWork.Permissions.SyncRolePermissionsAsync(roleId, quyenIds);

            if (!success) return BadRequest(new { message = "Có lỗi xảy ra khi đồng bộ quyền." });

            await _unitOfWork.CompleteAsync();
            return Ok(new { message = "Cập nhật quyền cho vai trò thành công." });
        }

        // ===============================================
        // 5. XÓA VAI TRÒ
        // ===============================================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (id == 1) return BadRequest(new { message = "Không thể xóa vai trò mặc định của hệ thống (SuperAdmin)." });

            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null) return NotFound(new { message = "Không tìm thấy vai trò." });

            // Kiểm tra xem có người dùng nào đang giữ vai trò này không
            var hasUsers = await _unitOfWork.Users.GetQueryable().AnyAsync(u => u.VaiTroID == id);
            if (hasUsers) return BadRequest(new { message = "Không thể xóa vai trò đang có người dùng sử dụng." });

            _unitOfWork.Roles.Delete(role);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Đã xóa vai trò thành công." });
        }
    }
}