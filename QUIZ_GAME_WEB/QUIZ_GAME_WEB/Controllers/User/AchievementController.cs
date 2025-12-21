using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;     // IResultRepository, IRewardService
using QUIZ_GAME_WEB.Models.ResultsModels;  // ThanhTuu, ChuoiNgay
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    // Đổi tên Controller nếu cần (ví dụ: AchievmentsController nếu bạn đặt tên lớp như vậy)
    public class AchievementController : ControllerBase
    {
        private readonly IResultRepository _resultRepo;
        private readonly IRewardService _rewardService;

        public AchievementController(IResultRepository resultRepo, IRewardService rewardService)
        {
            _resultRepo = resultRepo;
            _rewardService = rewardService;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        // ======================================================
        // 1. LẤY THÀNH TỰU ĐÃ ĐẠT (GET: api/user/achievement/me)
        // ======================================================
        /// <summary>
        /// Lấy danh sách thành tựu của chính người dùng
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAchievements()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                // Giả định GetUserAchievementsAsync trả về List<ThanhTuu> Entity hoặc DTO
                var list = await _resultRepo.GetUserAchievementsAsync(userId.Value);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy danh sách thành tựu: " + ex.Message });
            }
        }

        // ======================================================
        // 2. LẤY CHUỖI NGÀY CHƠI (GET: api/user/achievement/streak)
        // ======================================================
        /// <summary>
        /// Lấy thông tin chuỗi ngày chơi (streak)
        /// </summary>
        [HttpGet("streak")]
        public async Task<IActionResult> GetMyStreak()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                var streak = await _resultRepo.GetUserStreakAsync(userId.Value);

                if (streak == null)
                    // Trả về Streak 0 ngày
                    return Ok(new { soNgayLienTiep = 0, ngayCapNhatCuoi = (DateTime?)null });

                return Ok(new
                {
                    // streak.UserID, // Không cần thiết lộ UserID
                    streak.SoNgayLienTiep,
                    streak.NgayCapNhatCuoi
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy chuỗi ngày chơi: " + ex.Message });
            }
        }

        // ======================================================
        // 3. NHẬN THƯỞNG HẰNG NGÀY (POST: api/user/achievement/daily-reward)
        // ======================================================
        /// <summary>
        /// Nhận thưởng điểm đăng nhập hằng ngày
        /// </summary>
        [HttpPost("daily-reward")]
        public async Task<IActionResult> ClaimDailyReward()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                // Giả định CheckAndAwardDailyRewardAsync xử lý logic kiểm tra ngày và cộng điểm
                var awarded = await _rewardService.CheckAndAwardDailyRewardAsync(userId.Value);

                if (!awarded)
                    return Ok(new { awarded = false, message = "Hôm nay bạn đã nhận thưởng rồi hoặc không đủ điều kiện." });

                return Ok(new { awarded = true, message = "Nhận thưởng hằng ngày thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi yêu cầu nhận thưởng: " + ex.Message });
            }
        }
    }
}