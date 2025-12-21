using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data; // <== SỬA TẠI ĐÂY
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class BaoCaoController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public BaoCaoController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/admin/BaoCao/GamesByDate
        // Chức năng: Báo cáo số trận chơi theo ngày (90 ngày gần nhất)
        [HttpGet("GamesByDate")]
        public async Task<ActionResult> GetGamesPlayedByDate()
        {
            var report = await _context.KetQuas
                .GroupBy(k => k.ThoiGian.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    SoTran = g.Count(),
                    TongDiem = g.Sum(k => k.Diem),
                    SoCauDung = g.Sum(k => k.SoCauDung)
                })
                .OrderByDescending(r => r.Ngay)
                .Take(90)
                .ToListAsync();

            return Ok(report);
        }

        // GET: api/admin/BaoCao/UserRegistrationByDate
        // Chức năng: Báo cáo người dùng đăng ký mới theo ngày
        [HttpGet("UserRegistrationByDate")]
        public async Task<ActionResult> GetUserRegistrationByDate()
        {
            var report = await _context.NguoiDungs
               .GroupBy(u => u.NgayDangKy.Date)
               .Select(g => new
               {
                   Ngay = g.Key,
                   SoNguoiDungMoi = g.Count()
               })
               .OrderByDescending(r => r.Ngay)
               .ToListAsync();

            return Ok(report);
        }

        // GET: api/admin/BaoCao/TopWrongAnswers
        // Chức năng: Báo cáo 10 câu hỏi người dùng sai nhiều nhất
        [HttpGet("TopWrongAnswers")]
        public async Task<ActionResult> GetTopWrongAnswers()
        {
            var report = await _context.CauSais
                .GroupBy(cs => cs.CauHoiID)
                .Select(g => new
                {
                    CauHoiID = g.Key,
                    SoLanSai = g.Count(),
                    // Lấy nội dung câu hỏi
                    NoiDungCauHoi = _context.CauHois.Where(ch => ch.CauHoiID == g.Key).Select(ch => ch.NoiDung).FirstOrDefault()
                })
                .OrderByDescending(r => r.SoLanSai)
                .Take(10)
                .ToListAsync();

            return Ok(report);
        }
    }
}