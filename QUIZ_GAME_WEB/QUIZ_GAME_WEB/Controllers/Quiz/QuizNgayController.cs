using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels; // Giả định QuizNgayDetailsDto, KetQuaDto tồn tại
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class QuizNgayController : ControllerBase
    {
        private readonly IQuizAttemptService _quizAttemptService;
        private readonly IQuizRepository _quizRepository;

        public QuizNgayController(
            IQuizAttemptService quizAttemptService,
            IQuizRepository quizRepository)
        {
            _quizAttemptService = quizAttemptService;
            _quizRepository = quizRepository;
        }

        private int GetUserIdFromClaim()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID không hợp lệ trong token.");
            }
            return userId;
        }

        // ============================================
        // 1. LẤY QUIZ CỦA NGÀY HÔM NAY
        // ============================================
        [HttpGet("today")]
        [AllowAnonymous] // Cho phép xem câu hỏi (nhưng không thể chơi nếu chưa login)
        public async Task<IActionResult> GetTodayQuiz()
        {
            try
            {
                // ✅ Sử dụng hàm Repository mới để lấy chi tiết QuizNgay
                var quizDetails = await _quizRepository.GetTodayQuizDetailsAsync();

                if (quizDetails == null)
                {
                    return NotFound(new { message = "Chưa có Quiz Ngày nào được thiết lập hôm nay." });
                }

                return Ok(quizDetails); // Trả về QuizNgayDetailsDto (có nội dung câu hỏi)
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy Quiz Ngày: " + ex.Message });
            }
        }

        // ============================================
        // 2. BẮT ĐẦU LÀM QUIZ NGÀY
        // ============================================
        [HttpPost("start")]
        public async Task<IActionResult> StartTodayQuiz()
        {
            try
            {
                int userId = GetUserIdFromClaim();

                // 1. Lấy QuizNgay hôm nay để tìm CauHoiID
                var quizDetails = await _quizRepository.GetTodayQuizDetailsAsync();
                if (quizDetails == null)
                {
                    return NotFound(new { message = "Không tìm thấy Quiz Ngày hôm nay." });
                }
                int cauHoiId = quizDetails.CauHoiID;

                // 2. Gọi Service chuyên biệt để bắt đầu phiên (chỉ với 1 câu hỏi cố định)
                // Giả định StartDailyQuizAttemptAsync tồn tại trong IQuizAttemptService
                int attemptId = await _quizAttemptService.StartDailyQuizAttemptAsync(userId, cauHoiId);

                // 3. Lấy câu hỏi đầu tiên (chính là câu hỏi của Quiz Ngày)
                var firstQuestion = await _quizAttemptService.GetNextQuestionAsync(attemptId);

                return Ok(new
                {
                    attemptID = attemptId,
                    question = firstQuestion,
                    message = "Bắt đầu Quiz Ngày thành công."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi bắt đầu Quiz ngày: " + ex.Message });
            }
        }

        // ============================================
        // 3. NỘP ĐÁP ÁN QUIZ NGÀY
        // ============================================
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAnswerForToday([FromBody] AnswerSubmitModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                model.UserID = GetUserIdFromClaim();

                // ✅ Tái sử dụng hàm SubmitAnswerAsync chung của QuizAttemptService
                bool isCorrect = await _quizAttemptService.SubmitAnswerAsync(model);

                return Ok(new { isCorrect = isCorrect, message = "Đáp án đã được ghi nhận." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi nộp đáp án: " + ex.Message });
            }
        }

        // ============================================
        // 4. KẾT THÚC QUIZ NGÀY & XEM KẾT QUẢ
        // ============================================
        [HttpPost("end/{attemptId:int}")]
        public async Task<IActionResult> EndTodayQuiz(int attemptId)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                // 1. Gọi Service để kết thúc và lưu kết quả
                var resultEntity = await _quizAttemptService.EndAttemptAndCalculateResultAsync(attemptId, userId);

                // 2. Map sang DTO (giống ChoiController.End)
                var resultDto = new KetQuaDto
                {
                    QuizAttemptID = resultEntity.QuizAttemptID,
                    Diem = resultEntity.Diem,
                    SoCauDung = resultEntity.SoCauDung,
                    TongCauHoi = resultEntity.TongCauHoi,
                    TrangThaiKetQua = resultEntity.TrangThaiKetQua
                };

                return Ok(resultDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi kết thúc Quiz ngày: " + ex.Message });
            }
        }
    }
}