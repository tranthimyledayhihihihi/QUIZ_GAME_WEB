using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using System.Security.Claims;

[Route("api/trandau")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class TranDauController : ControllerBase
{
    private readonly IOnlineMatchService _service;

    public TranDauController(IOnlineMatchService service)
    {
        _service = service;
    }

    private int GetUserId()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(id);
    }

    [HttpGet("{matchCode}")]
    public async Task<IActionResult> GetMatch(string matchCode)
    {
        // 1. Lấy thông tin trận
        var match = await _service.GetMatchByCodeAsync(matchCode);
        if (match == null) return NotFound();
        // 2. Lấy danh sách câu hỏi (đã sửa ở bước trên)
        var questions = await _service.GetQuestionsByMatchCodeAsync(matchCode);
        // 3. Trả về đối tượng gộp (Anonymous Object)
        return Ok(new
        {
            // Copy các thuộc tính của match
            match.TranDauID,
            match.MatchCode,
            match.Player1ID,
            match.Player2ID,
            match.TrangThai,
            match.DiemPlayer1,
            match.DiemPlayer2,
            match.WinnerUserID,

            // Kèm thêm danh sách câu hỏi
            Questions = questions
        });
    }

    [HttpPost("gui-dap-an/{matchCode}")]
    public async Task<IActionResult> SubmitAnswer(string matchCode, [FromBody] MatchAnswerModel model)
    {
        int id = GetUserId();
        bool ok = await _service.SubmitAnswerByMatchCodeAsync(matchCode, id, model);
        if (!ok) return BadRequest();

        return NoContent();
    }

    [HttpPost("ket-thuc/{matchCode}")]
    public async Task<IActionResult> EndMatch(string matchCode)
    {
        var result = await _service.EndMatchByCodeAsync(matchCode);
        return Ok(result);
    }
}
