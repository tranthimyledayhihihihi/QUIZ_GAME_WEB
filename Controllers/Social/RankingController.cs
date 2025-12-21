using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels; // Cần DTOs cho BXH
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Social
{
    [Route("api/[controller]")]
    [ApiController]
    // BXH thường là Public (AllowAnonymous) hoặc chỉ yêu cầu đăng nhập để xem vị trí cá nhân
    public class RankingController : ControllerBase
    {
        private readonly IResultRepository _resultRepository;
        private readonly ISocialRepository _socialRepository; // Giả định Repository cho BXH/Social

        public RankingController(IResultRepository resultRepository, ISocialRepository socialRepository)
        {
            _resultRepository = resultRepository;
            _socialRepository = socialRepository;
        }

        private int? GetUserIdFromClaim()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out int userId))
            {
                return userId;
            }
            return null;
        }

        // ======================================================
        // 1. BXH THEO THỜI GIAN (TUẦN/THÁNG)
        // ======================================================

        /// <summary>
        /// Lấy Bảng Xếp Hạng (BXH) theo tuần hoặc tháng.
        /// GET: api/Ranking/leaderboard?type=weekly
        /// </summary>
        [HttpGet("leaderboard")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLeaderboard(
            [FromQuery] string type = "monthly",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Giả định ISocialRepository có hàm GetLeaderboardAsync
                // và trả về DTOs phù hợp (ví dụ: RankingDto)
                var (ranking, totalCount) = await _socialRepository.GetLeaderboardAsync(
                    type, pageNumber, pageSize);

                return Ok(new
                {
                    type = type,
                    tongSoNguoi = totalCount,
                    trangHienTai = pageNumber,
                    tongSoTrang = (int)Math.Ceiling(totalCount / (double)pageSize),
                    danhSach = ranking
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy Bảng xếp hạng: " + ex.Message });
            }
        }

        // ======================================================
        // 2. THÀNH TỰU CỦA TÔI
        // ======================================================

        /// <summary>
        /// Lấy danh sách các Thành tựu người dùng đã đạt được.
        /// GET: api/Ranking/achievements/my
        /// </summary>
        [HttpGet("achievements/my")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetMyAchievements()
        {
            try
            {
                int? userId = GetUserIdFromClaim();
                if (userId == null) return Unauthorized();

                // ✅ Sử dụng IResultRepository (đã có)
                var achievements = await _resultRepository.GetUserAchievementsAsync(userId.Value);

                if (!achievements.Any())
                {
                    return Ok(new { message = "Bạn chưa đạt được thành tựu nào." });
                }

                return Ok(achievements); // Giả định trả về List<ThanhTuu> Entity hoặc DTO
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy thành tựu: " + ex.Message });
            }
        }

        // ======================================================
        // 3. XEM THÔNG TIN ONLINE (Tùy chọn)
        // ======================================================

        /// <summary>
        /// Lấy tổng số người dùng đang online (Tùy chọn)
        /// GET: api/Ranking/online-count
        /// </summary>
        [HttpGet("online-count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOnlineUserCount()
        {
            try
            {
                // Giả định ISocialRepository có hàm CountOnlineUsersAsync
                var count = await _socialRepository.CountOnlineUsersAsync();

                return Ok(new { tongNguoiOnline = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi đếm người dùng online: " + ex.Message });
            }
        }
    }
}