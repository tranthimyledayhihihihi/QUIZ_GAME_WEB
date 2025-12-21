using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    public class CauHoiController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;

        public CauHoiController(IQuizRepository quizRepository)
        {
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

        // ========================================================
        // 1. API LẤY CÂU HỎI SAI ĐỂ ÔN TẬP (Endpoint chính)
        // ========================================================
        [HttpGet("incorrect-review")]
        [Authorize(AuthenticationSchemes = "Bearer")] // Bắt buộc đăng nhập
        public async Task<IActionResult> GetIncorrectQuestionsForReview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                var (questions, totalCount) = await _quizRepository.GetIncorrectQuestionsByUserIdAsync(
                    userId, pageNumber, pageSize);

                if (totalCount == 0)
                {
                    return Ok(new { message = "Xin chúc mừng! Bạn chưa có câu hỏi nào trả lời sai cần ôn tập.", tongSoCauHoiSai = 0 });
                }

                return Ok(new
                {
                    tongSoCauHoiSai = totalCount,
                    trangHienTai = pageNumber,
                    kichThuocTrang = pageSize,
                    tongSoTrang = (int)Math.Ceiling(totalCount / (double)pageSize),
                    danhSach = questions
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập để xem danh sách ôn tập." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy danh sách câu hỏi sai: " + ex.Message });
            }
        }

        // ========================================================
        // 2. CÁC API THỐNG KÊ VÀ TỔNG HỢP (Giữ Public)
        // ========================================================

        [HttpGet("total-count")]
        public async Task<IActionResult> GetTotalQuestionsCount()
        {
            try
            {
                var totalCount = await _quizRepository.CountAllCauHoisAsync();
                return Ok(new { tongSoCauHoi = totalCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetQuestionStatistics()
        {
            try
            {
                var allQuestions = await _quizRepository.GetAllCauHoisWithDetailsAsync();

                var statsByChuDe = allQuestions
                    .GroupBy(q => q.ChuDe?.TenChuDe ?? "Chưa phân loại")
                    .Select(g => new { tenChuDe = g.Key, soLuong = g.Count() })
                    .ToList();

                var statsByDoKho = allQuestions
                    .GroupBy(q => q.DoKho?.TenDoKho ?? "Chưa phân loại")
                    .Select(g => new { tenDoKho = g.Key, soLuong = g.Count() })
                    .ToList();

                return Ok(new
                {
                    tongSoCauHoi = allQuestions.Count(),
                    theoGiaDe = statsByChuDe,
                    theoDoKho = statsByDoKho
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // *LƯU Ý: Các API công cộng GetRandom, Search, GetByChuDe, GetByDoKho đã bị loại bỏ theo yêu cầu.*
    }
}