using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/trandau")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TranDauController : ControllerBase
    {
        private readonly IOnlineMatchService _service;
        private readonly ISocketGameServer _socketServer;

        public TranDauController(
            IOnlineMatchService service,
            ISocketGameServer socketServer)
        {
            _service = service;
            _socketServer = socketServer;
        }

        // =====================================================
        // HELPER
        // =====================================================
        private int GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(id, out var uid) ? uid : 0;
        }

        // =====================================================
        // CREATE MATCH (API) - TẠO PHÒNG RIÊNG
        // =====================================================
        [HttpPost("create")]
        public async Task<IActionResult> CreateMatch()
        {
            int userId = GetUserId();
            if (userId == 0)
                return Unauthorized("User không hợp lệ");

            string matchCode = await _service.CreateMatchAsync(userId);

            return Ok(new
            {
                matchCode,
                message = "Phòng đã được tạo. Kết nối WebSocket để chờ người chơi join."
            });
        }

        // =====================================================
        // JOIN MATCH (API) - NHẬP MÃ PHÒNG
        // =====================================================
        [HttpPost("join/{matchCode}")]
        public async Task<IActionResult> JoinMatch(string matchCode)
        {
            int userId = GetUserId();
            if (userId == 0)
                return Unauthorized("User không hợp lệ");

            var match = await _service.GetMatchByCodeAsync(matchCode);

            if (match == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            if (match.TrangThai != "DangCho")
                return BadRequest(new { message = "Phòng đã bắt đầu hoặc đã kết thúc" });

            if (match.Player2ID != 0)
                return BadRequest(new { message = "Phòng đã đầy" });

            if (match.Player1ID == userId)
                return BadRequest(new { message = "Bạn là chủ phòng" });

            // 🔥 CHỈ UPDATE DB – KHÔNG BROADCAST
            match.Player2ID = userId;
            match.TrangThai = "DangChoi";
            match.ThoiGianBatDau = DateTime.Now;

            await _service.UpdateMatchAsync(match);

            return Ok(new
            {
                matchCode,
                message = "Đã vào phòng. WebSocket sẽ xử lý realtime."
            });
        }

        // =====================================================
        // MATCH STATUS (API)
        // =====================================================
        [HttpGet("status/{matchCode}")]
        public async Task<IActionResult> GetMatchStatus(string matchCode)
        {
            var match = await _service.GetMatchByCodeAsync(matchCode);

            if (match == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            return Ok(new
            {
                matchCode = match.MatchCode,
                trangThai = match.TrangThai,
                player1Id = match.Player1ID,
                player2Id = match.Player2ID,
                isWaitingForPlayer = match.Player2ID == 0,
                isPlaying = match.TrangThai == "DangChoi",
                isFinished = match.TrangThai == "HoanThanh"
            });
        }

        // =====================================================
        // MATCH HISTORY (API)
        // =====================================================
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            int userId = GetUserId();
            if (userId == 0)
                return Unauthorized();

            var history = await _service.GetMatchHistoryAsync(userId);
            return Ok(history);
        }

        // =====================================================
        // MATCH DETAIL (API)
        // =====================================================
        [HttpGet("detail/{matchCode}")]
        public async Task<IActionResult> GetMatchDetail(string matchCode)
        {
            int userId = GetUserId();
            if (userId == 0)
                return Unauthorized();

            var match = await _service.GetMatchByCodeAsync(matchCode);

            if (match == null)
                return NotFound();

            if (match.Player1ID != userId && match.Player2ID != userId)
                return Forbid();

            return Ok(match);
        }

        // =====================================================
        // ONLINE COUNT (API)
        // =====================================================
        [HttpGet("online-count")]
        [AllowAnonymous]
        public IActionResult GetOnlineCount()
        {
            int count = _socketServer.GetOnlineCount();
            return Ok(new { onlineUsers = count });
        }

        // =====================================================
        // CANCEL MATCH (API)
        // =====================================================
        [HttpDelete("cancel/{matchCode}")]
        public async Task<IActionResult> CancelMatch(string matchCode)
        {
            int userId = GetUserId();
            if (userId == 0)
                return Unauthorized();

            var match = await _service.GetMatchByCodeAsync(matchCode);

            if (match == null)
                return NotFound();

            if (match.Player1ID != userId)
                return Forbid();

            if (match.TrangThai != "DangCho")
                return BadRequest(new { message = "Không thể hủy phòng đã bắt đầu" });

            match.TrangThai = "Huy";
            await _service.UpdateMatchAsync(match);

            return Ok(new { message = "Đã hủy phòng" });
        }
    }
}
