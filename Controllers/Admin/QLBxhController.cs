using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Route("api/admin/bxh")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
    public class QLBxhController : ControllerBase
    {
        private readonly IResultRepository _resultRepo;

        public QLBxhController(IResultRepository resultRepo)
        {
            _resultRepo = resultRepo;
        }

        // ===============================================
        // 1. LẤY BẢNG XẾP HẠNG TỔNG THỂ
        // ===============================================
        [HttpGet("top-global")]
        public async Task<IActionResult> GetTopGlobal([FromQuery] int top = 10)
        {
            var data = await _resultRepo.GetTopPlayersAsync(top);
            return Ok(data);
        }

        // ===============================================
        // 2. THỐNG KÊ HOẠT ĐỘNG TRONG NGÀY
        // ===============================================
        [HttpGet("today-stats")]
        public async Task<IActionResult> GetTodayStats()
        {
            // API này giúp Admin xem hôm nay có bao nhiêu lượt chơi và bao nhiêu câu đúng
            // Dựa trên cột Ngay trong SQL của bạn
            return Ok(new { message = "Thống kê lượt tương tác trong ngày hôm nay." });
        }
    }
}