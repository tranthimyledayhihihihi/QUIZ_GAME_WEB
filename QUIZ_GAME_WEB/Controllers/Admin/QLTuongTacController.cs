using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.SocialRankingModels; // Namespace của Comment
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Route("api/admin/tuong-tac")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
    public class QLTuongTacController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QLTuongTacController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===============================================
        // 1. QUẢN LÝ BÌNH LUẬN (Comment)
        // ===============================================

        [HttpGet("binh-luan")]
        public async Task<IActionResult> GetAllComments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var repo = _unitOfWork.Quiz.GetCommentRepository();

            // Sử dụng các tên thuộc tính chính xác từ Model: Content, NgayTao, User
            var query = repo.GetQueryable()
                            .Include(c => c.User)
                            .OrderByDescending(c => c.NgayTao);

            var total = await query.CountAsync();
            var data = await query.Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .Select(c => new {
                                      c.CommentID,
                                      c.Content,
                                      c.NgayTao,
                                      c.EntityType,      // Phân loại bình luận (Câu hỏi, Quiz...)
                                      c.RelatedEntityID,  // ID của đối tượng được bình luận
                                      Username = c.User != null ? c.User.TenDangNhap : "Ẩn danh"
                                  })
                                  .ToListAsync();

            return Ok(new { total, data });
        }

        [HttpDelete("binh-luan/{id:int}")]
        [Authorize(Roles = "SuperAdmin")] // Chỉ SuperAdmin mới có quyền xóa vĩnh viễn
        public async Task<IActionResult> DeleteComment(int id)
        {
            var repo = _unitOfWork.Quiz.GetCommentRepository();
            var comment = await repo.GetByIdAsync(id);

            if (comment == null)
                return NotFound(new { message = "Không tìm thấy bình luận." });

            repo.Delete(comment);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Đã xóa bình luận thành công." });
        }

        // ===============================================
        // 2. THEO DÕI CHIA SẺ (QuizChiaSe)
        // ===============================================

        [HttpGet("lich-su-chia-se")]
        public async Task<IActionResult> GetShareHistory()
        {
            // Tận dụng hàm đã có trong IQuizRepository để lấy dữ liệu chia sẻ
            // Ở đây Admin có thể xem tổng quan hoặc lọc theo User
            return Ok(new { message = "Chức năng xem lịch sử chia sẻ toàn hệ thống." });
        }
    }
}