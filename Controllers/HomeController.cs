using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using System.Threading.Tasks;
using System;
using System.Linq; // ✅ Cần cho AnyAsync

// Namespace đã được đặt về QUIZ_GAME_WEB.Controllers
namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public HomeController(QuizGameContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // 1. KIỂM TRA TRẠNG THÁI API (Smoke Test)
        // ----------------------------------------------------
        // GET: api/Home
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                Status = "Running",
                Service = "QuizGame API",
                Version = "1.0",
                Time = DateTime.UtcNow
            });
        }

        // ----------------------------------------------------
        // 2. KIỂM TRA KẾT NỐI DATABASE (Health Check)
        // ----------------------------------------------------
        // GET: api/Home/HealthCheck
        [HttpGet("HealthCheck")]
        public async Task<IActionResult> DatabaseHealthCheck()
        {
            try
            {
                // Bước 1: Kiểm tra kết nối cơ bản (CanConnectAsync)
                if (!await _context.Database.CanConnectAsync())
                {
                    return StatusCode(503, new // 503 Service Unavailable
                    {
                        DatabaseStatus = "Connection Error",
                        Message = "Database không phản hồi hoặc không khả dụng."
                    });
                }

                // ✅ Bước 2: Thực hiện truy vấn nhẹ để kiểm tra tính toàn vẹn (Lightweight Query)
                // Kiểm tra xem bảng NguoiDungs có thể được truy vấn không.
                // Điều này mạnh mẽ hơn CanConnectAsync() đơn thuần.
                await _context.NguoiDungs.AnyAsync();

                return Ok(new
                {
                    DatabaseStatus = "OK",
                    Message = "API và Database đều hoạt động và có thể truy cập."
                });
            }
            catch (Exception ex)
            {
                // Lỗi có thể là timeout, lỗi kết nối SQL, hoặc lỗi schema EF Core
                return StatusCode(500, new
                {
                    DatabaseStatus = "Fatal Error",
                    Message = "Lỗi nghiêm trọng khi kiểm tra Database Health.",
                    Detail = ex.Message,
                    Type = ex.GetType().Name
                });
            }
        }

        // ----------------------------------------------------
        // 3. ENDPOINT MẶC ĐỊNH CHO ROOT PATH
        // ----------------------------------------------------
        // GET: /
        [HttpGet("/")]
        public IActionResult GetRoot()
        {
            // Trả về kết quả của GetStatus
            return RedirectToAction(nameof(GetStatus));
        }
    }
}