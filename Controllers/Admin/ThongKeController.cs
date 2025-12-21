using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data; // <== SỬA TẠI ĐÂY
using QUIZ_GAME_WEB.Models;
using QUIZ_GAME_WEB.Models.ResultsModels; // ThongKeNguoiDung, ChuoiNgay

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ThongKeController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/ThongKe/User/{userId}/Daily
        // Chức năng: Lấy thống kê chơi game hàng ngày của một người dùng (30 ngày gần nhất)
        [HttpGet("User/{userId}/Daily")]
        public async Task<ActionResult<IEnumerable<ThongKeNguoiDung>>> GetUserDailyStats(int userId)
        {
            // Logic nghiệp vụ: Lọc theo thời gian và người dùng
            var startDate = DateTime.Today.AddDays(-30);

            return await _context.ThongKeNguoiDungs
                .Where(tk => tk.UserID == userId && tk.Ngay >= startDate)
                .OrderByDescending(tk => tk.Ngay)
                .ToListAsync();
        }

        // GET: api/ThongKe/User/{userId}/Streak
        // Chức năng: Lấy chuỗi ngày liên tiếp chơi game
        [HttpGet("User/{userId}/Streak")]
        public async Task<ActionResult<ChuoiNgay>> GetUserStreak(int userId)
        {
            var chuoiNgay = await _context.ChuoiNgays.FirstOrDefaultAsync(cn => cn.UserID == userId);

            if (chuoiNgay == null)
            {
                // Logic nghiệp vụ: Trả về đối tượng ChuoiNgay mặc định nếu chưa tồn tại
                return NotFound(new ChuoiNgay { UserID = userId, SoNgayLienTiep = 0, NgayCapNhatCuoi = DateTime.Now.Date });
            }

            return chuoiNgay;
        }
    }
}