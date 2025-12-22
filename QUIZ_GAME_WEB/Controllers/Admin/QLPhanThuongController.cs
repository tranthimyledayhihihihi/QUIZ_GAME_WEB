using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels; // ThuongNgay (Đảm bảo đúng Namespace)
using QUIZ_GAME_WEB.Models.ResultsModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Route("api/admin/phan-thuong")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin")]
    public class QLPhanThuongController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QLPhanThuongController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===============================================
        // 1. LẤY DANH SÁCH LỊCH SỬ NHẬN THƯỞNG (PHÂN TRANG)
        // ===============================================
        [HttpGet("lich-su")]
        public async Task<IActionResult> GetRewardHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var repo = _unitOfWork.Quiz.GetRewardRepository();

            var query = repo.GetQueryable()
                            .Include(t => t.NguoiDung) // Để lấy tên người nhận
                            .OrderByDescending(t => t.NgayNhan);

            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .Select(t => new {
                                      t.ThuongID,
                                      Username = t.NguoiDung.TenDangNhap,
                                      t.PhanThuong,
                                      t.DiemThuong,
                                      t.NgayNhan,
                                      t.TrangThaiNhan
                                  })
                                  .ToListAsync();

            return Ok(new { total, data });
        }

        // ===============================================
        // 2. TẠO PHẦN THƯỞNG CHO NGƯỜI DÙNG CỤ THỂ
        // ===============================================
        // Trong QLPhanThuongController.cs

        [HttpPost("tang-qua")]
        public async Task<IActionResult> GiveReward([FromBody] ThuongNgayInputModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Kiểm tra người dùng tồn tại
            var user = await _unitOfWork.Users.GetByIdAsync(model.UserID);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng." });

            // 2. Ánh xạ (Map) từ InputModel sang Entity chính
            var reward = new ThuongNgay
            {
                ThuongID = 0, // Để SQL tự tăng
                UserID = model.UserID,
                PhanThuong = model.PhanThuong,
                DiemThuong = model.DiemThuong,
                TrangThaiNhan = model.TrangThaiNhan,
                NgayNhan = DateTime.Now // Tự động lấy ngày hiện tại
            };

            var repo = _unitOfWork.Quiz.GetRewardRepository();
            repo.Add(reward);

            await _unitOfWork.CompleteAsync();

            return Ok(new
            {
                message = $"Đã tặng quà cho {user.TenDangNhap} thành công!",
                id = reward.ThuongID
            });
        }

        // ===============================================
        // 3. XÓA BẢN GHI THƯỞNG (Dọn dẹp log)
        // ===============================================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReward(int id)
        {
            var repo = _unitOfWork.Quiz.GetRewardRepository();
            var item = await repo.GetByIdAsync(id);

            if (item == null) return NotFound();

            repo.Delete(item);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}