using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;       // IResultRepository
using QUIZ_GAME_WEB.Models.ResultsModels;    // CauSai
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/quiz/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CauSaiController : ControllerBase
    {
        private readonly IResultRepository _resultRepo;

        public CauSaiController(IResultRepository resultRepo)
        {
            _resultRepo = resultRepo;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        /// <summary>
        /// Lấy danh sách các câu sai gần đây của người dùng
        /// GET: api/quiz/causai/recent?limit=10
        /// </summary>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentWrongAnswers([FromQuery] int limit = 10)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            if (limit <= 0) limit = 10;

            var list = await _resultRepo.GetRecentWrongAnswersAsync(userId.Value, limit);
            return Ok(list);
        }

        /// <summary>
        /// Đếm số câu sai trong một lần làm bài (QuizAttempt)
        /// GET: api/quiz/causai/count/{attemptId}
        /// </summary>
        [HttpGet("count/{attemptId:int}")]
        public async Task<IActionResult> CountWrongAnswers(int attemptId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            var count = await _resultRepo.CountWrongAnswersAsync(userId.Value, attemptId);
            return Ok(new { attemptId, count });
        }
    }
}
