using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChoiController : ControllerBase
    {
        private readonly IQuizAttemptService _quizService;
        // Loại bỏ trường cấp lớp này, vì nó không được sử dụng đúng cách
        // private int userId; 

        public ChoiController(IQuizAttemptService quizService)
        {
            _quizService = quizService;
        }

        // Lấy UserID từ JWT
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID không hợp lệ.");
            }
            return userId;
        }

        // --- 1. Bắt đầu làm bài (POST /api/choi/start) ---
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] GameStartOptions options)
        {
            try
            {
                int userId = GetUserId();
                int attemptId = await _quizService.StartNewQuizAttemptAsync(userId, options);
                var firstQuestion = await _quizService.GetNextQuestionAsync(attemptId);

                if (firstQuestion == null)
                {
                    return NotFound(new { message = "Không tìm thấy câu hỏi để bắt đầu." });
                }

                return Ok(new
                {
                    attemptID = attemptId,
                    question = firstQuestion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- 2. Nộp đáp án (POST /api/choi/submit) ---
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAnswer([FromBody] AnswerSubmitModel answer)
        {
            try
            {
                answer.UserID = GetUserId();
                bool isCorrect = await _quizService.SubmitAnswerAsync(answer);
                return Ok(new { isCorrect = isCorrect, message = "Đáp án đã được ghi nhận." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- 3. Kết thúc và xem kết quả (POST /api/choi/end/{attemptId}) ---
        [HttpPost("end/{attemptId}")]
        public async Task<IActionResult> End(int attemptId)
        {
            try
            {
                // ✅ Lấy UserID từ JWT (Như trong hàm Start)
                int userId = GetUserId();

                // Service trả về KetQua Entity (giả định đã khắc phục lỗi JSON Cycle)
                var result = await _quizService.EndAttemptAndCalculateResultAsync(attemptId, userId);

                // ✅ Sử dụng AutoMapper hoặc Map thủ công sang DTO trước khi trả về
                // Giả định KetQuaDto là một ViewModel đã được định nghĩa
                var resultDto = new KetQuaDto
                {
                    QuizAttemptID = result.QuizAttemptID,
                    Diem = result.Diem,
                    SoCauDung = result.SoCauDung,
                    TongCauHoi = result.TongCauHoi,
                    TrangThaiKetQua = result.TrangThaiKetQua
                };

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                // ✅ Thêm khối try-catch để xử lý lỗi session/DB
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- 4. Lấy câu hỏi tiếp theo (GET /api/choi/next/{attemptId}) ---
        [HttpGet("next/{attemptId}")]
        public async Task<IActionResult> GetNextQuestion(int attemptId)
        {
            var question = await _quizService.GetNextQuestionAsync(attemptId);

            if (question == null)
                return NotFound(new { message = "Không còn câu hỏi nào để trả lời. Vui lòng kết thúc phiên làm bài." });

            return Ok(question);
        }
    }
}