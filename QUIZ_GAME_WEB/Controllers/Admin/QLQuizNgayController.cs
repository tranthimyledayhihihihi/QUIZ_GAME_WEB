using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.QuizModels; // QuizNgay
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Route("api/admin/quiz-ngay")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
    public class QLQuizNgayController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QLQuizNgayController(QuizGameContext context)
        {
            _context = context;
        }

        // ===============================================
        // 1. LẤY DANH SÁCH LỊCH TRÌNH QUIZ NGÀY
        // ===============================================
        [HttpGet("lich-trinh")]
        public async Task<IActionResult> GetSchedule([FromQuery] int month, [FromQuery] int year)
        {
            var schedule = await _context.QuizNgays
                .Include(q => q.CauHoi) // Load thông tin câu hỏi để Admin dễ xem
                .Where(q => q.Ngay.Month == month && q.Ngay.Year == year)
                .OrderBy(q => q.Ngay)
                .Select(q => new {
                    q.QuizNgayID,
                    q.Ngay,
                    q.CauHoiID,
                    NoiDungCauHoi = q.CauHoi.NoiDung
                })
                .ToListAsync();

            return Ok(schedule);
        }

        // ===============================================
        // 2. THIẾT LẬP CÂU HỎI CHO MỘT NGÀY CỤ THỂ
        // ===============================================
        [HttpPost("set-daily")]
        public async Task<IActionResult> SetDailyQuiz([FromBody] QuizNgayInput model)
        {
            // Kiểm tra xem ngày này đã có Quiz chưa
            var existing = await _context.QuizNgays
                .FirstOrDefaultAsync(q => q.Ngay.Date == model.Ngay.Date);

            if (existing != null)
            {
                existing.CauHoiID = model.CauHoiID; // Cập nhật nếu đã tồn tại
                _context.QuizNgays.Update(existing);
            }
            else
            {
                var newQuiz = new QuizNgay
                {
                    Ngay = model.Ngay.Date,
                    CauHoiID = model.CauHoiID
                };
                _context.QuizNgays.Add(newQuiz);
            }

            await _context.CompleteAsync(); // Giả định dùng UnitOfWork hoặc _context.SaveChangesAsync()
            return Ok(new { message = $"Đã thiết lập câu hỏi ID {model.CauHoiID} cho ngày {model.Ngay:dd/MM/yyyy}" });
        }
    }

    public class QuizNgayInput
    {
        public DateTime Ngay { get; set; }
        public int CauHoiID { get; set; }
    }
}