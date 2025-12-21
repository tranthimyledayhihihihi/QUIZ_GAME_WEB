using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels; // Giả định KetQuaDto, KetQuaDetailDto nằm ở đây
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LichSuChoiController : ControllerBase
    {
        private readonly IResultRepository _resultRepository;

        public LichSuChoiController(IResultRepository resultRepository)
        {
            _resultRepository = resultRepository;
        }

        private int GetUserIdFromClaim()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int userId))
            {
                // Thay vì throw UnauthorizedAccessException, trong Controller nên trả về Unauthorized
                throw new UnauthorizedAccessException("User ID không hợp lệ trong token.");
            }
            return userId;
        }

        // ======================================================
        // 1. DANH SÁCH KẾT QUẢ CỦA TÔI (PHÂN TRANG)
        // GET: api/LichSuChoi/my?pageNumber=&pageSize=
        // ======================================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyResults(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                // ✅ Sử dụng hàm Repository mới để lấy danh sách kết quả có phân trang
                var (results, totalCount) = await _resultRepository.GetPaginatedResultsAsync(
                    userId, pageNumber, pageSize);

                if (totalCount == 0)
                {
                    return Ok(new { message = "Bạn chưa có bài làm nào trong lịch sử.", tongSoKetQua = 0 });
                }

                return Ok(new
                {
                    tongSoKetQua = totalCount,
                    trangHienTai = pageNumber,
                    kichThuocTrang = pageSize,
                    tongSoTrang = (int)Math.Ceiling(totalCount / (double)pageSize),
                    danhSach = results // Giả định đây là IEnumerable<KetQuaDto>
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy lịch sử kết quả: " + ex.Message });
            }
        }

        // ======================================================
        // 2. CHI TIẾT MỘT LẦN LÀM BÀI
        // GET: api/LichSuChoi/{attemptId}
        // ======================================================
        [HttpGet("{attemptId:int}")]
        public async Task<IActionResult> GetResultDetail(int attemptId)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                // ✅ Sử dụng hàm Repository mới để lấy chi tiết kết quả và kiểm tra quyền sở hữu
                var resultDetail = await _resultRepository.GetResultDetailByAttemptIdAsync(attemptId, userId);

                if (resultDetail == null)
                {
                    return NotFound(new { message = "Không tìm thấy chi tiết bài làm hoặc bạn không có quyền truy cập." });
                }

                return Ok(resultDetail); // Giả định đây là KetQuaDetailDto
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy chi tiết kết quả: " + ex.Message });
            }
        }

        // ======================================================
        // 3. STREAK (CHUỖI NGÀY CHƠI)
        // GET: api/LichSuChoi/streak
        // ======================================================
        [HttpGet("streak")]
        public async Task<IActionResult> GetMyStreak()
        {
            try
            {
                int userId = GetUserIdFromClaim();

                // ✅ Sử dụng hàm đã có trong IResultRepository
                var streak = await _resultRepository.GetUserStreakAsync(userId);

                if (streak == null)
                {
                    // Trả về đối tượng Streak mặc định/khởi tạo nếu chưa có
                    return Ok(new { SoNgayLienTiep = 0, message = "Bạn chưa bắt đầu chuỗi ngày chơi nào." });
                }

                return Ok(new
                {
                    streak.ChuoiID,
                    streak.UserID,
                    streak.SoNgayLienTiep,
                    streak.NgayCapNhatCuoi
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy chuỗi ngày chơi: " + ex.Message });
            }
        }

        // ======================================================
        // 4. THÀNH TỰU (ACHIEVEMENTS)
        // GET: api/LichSuChoi/achievements
        // ======================================================
        [HttpGet("achievements")]
        public async Task<IActionResult> GetMyAchievements()
        {
            try
            {
                int userId = GetUserIdFromClaim();

                // ✅ Sử dụng hàm đã có trong IResultRepository
                var achievements = await _resultRepository.GetUserAchievementsAsync(userId);

                if (achievements == null || !achievements.Any())
                {
                    return Ok(new { message = "Người dùng chưa đạt được thành tựu nào." });
                }

                return Ok(achievements); // Giả định trả về list các ThanhTuu Entity hoặc DTO
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy thành tựu: " + ex.Message });
            }
        }
    }
}