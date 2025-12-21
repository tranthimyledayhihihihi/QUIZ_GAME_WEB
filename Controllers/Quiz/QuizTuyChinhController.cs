using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq; // ✅ CẦN THIẾT cho Any()

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class QuizTuyChinhController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQuizRepository _quizRepository;

        public QuizTuyChinhController(IUnitOfWork unitOfWork, IQuizRepository quizRepository)
        {
            _unitOfWork = unitOfWork;
            _quizRepository = quizRepository;
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

        // =========================================================================
        // 1. LẤY DANH SÁCH ĐỀ XUẤT CỦA TÔI (GET /api/QuizTuyChinh/my-submissions)
        // =========================================================================
        [HttpGet("my-submissions")]
        public async Task<IActionResult> GetMySubmissions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                var (quizzes, totalCount) = await _unitOfWork.Quiz.GetQuizSubmissionsByUserIdAsync(
                    userId, pageNumber, pageSize);

                if (!quizzes.Any() && totalCount > 0)
                {
                    // Trường hợp trang không có dữ liệu nhưng tổng số lớn hơn 0 (ví dụ: request trang quá lớn)
                    return NotFound(new { message = "Không tìm thấy đề xuất nào trên trang này." });
                }

                return Ok(new
                {
                    tongSoDeXuat = totalCount,
                    trangHienTai = pageNumber,
                    tongSoTrang = (int)Math.Ceiling(totalCount / (double)pageSize),
                    danhSach = quizzes
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy danh sách đề xuất: " + ex.Message });
            }
        }

        // =========================================================================
        // 2. GỬI ĐỀ XUẤT QUIZ MỚI (POST /api/QuizTuyChinh/submit)
        // =========================================================================
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionModel submission)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                int userId = GetUserIdFromClaim();

                // Hàm SubmitNewQuizAsync sẽ tạo QuizTuyChinh và các CauHoi liên quan
                var result = await _unitOfWork.Quiz.SubmitNewQuizAsync(userId, submission);
                await _unitOfWork.CompleteAsync(); // Lưu tất cả thay đổi

                return StatusCode(201, new
                {
                    quizId = result.QuizTuyChinhID,
                    message = "Đề xuất đã được gửi thành công và đang chờ Ban Quản Trị phê duyệt.",
                    trangThai = result.TrangThai // Trả về trạng thái thực tế
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi gửi đề xuất Quiz: " + ex.Message });
            }
        }

        // =========================================================================
        // 3. XÓA ĐỀ XUẤT CỦA TÔI (DELETE /api/QuizTuyChinh/5)
        // =========================================================================
        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteSubmission(int quizId)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                // Hàm này sẽ kiểm tra quyền sở hữu và trạng thái "Pending" trước khi xóa
                bool success = await _unitOfWork.Quiz.DeleteQuizSubmissionAsync(quizId, userId);

                if (!success)
                {
                    return NotFound("Đề xuất không tồn tại, bạn không có quyền xóa, hoặc đã được duyệt/từ chối.");
                }

                await _unitOfWork.CompleteAsync(); // Lưu thay đổi xóa

                return Ok(new { message = "Xóa đề xuất thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi xóa đề xuất: " + ex.Message });
            }
        }

        // =========================================================================
        // 4. LẤY CHI TIẾT ĐỀ XUẤT (GET /api/QuizTuyChinh/5) - Để xem lại chi tiết trước khi duyệt
        // =========================================================================
        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetSubmissionDetails(int quizId)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                var quizDetail = await _unitOfWork.Quiz.GetQuizSubmissionByIdAsync(quizId);

                // Kiểm tra quyền sở hữu (Hoặc là SuperAdmin)
                if (quizDetail == null || quizDetail.UserID != userId)
                {
                    return NotFound("Đề xuất không tồn tại hoặc bạn không có quyền truy cập.");
                }

                // Cần ánh xạ QuizTuyChinh Entity sang Detail DTO nếu cần (đã bỏ qua bước này trong mock)
                return Ok(quizDetail);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy chi tiết đề xuất: " + ex.Message });
            }
        }
    }
}