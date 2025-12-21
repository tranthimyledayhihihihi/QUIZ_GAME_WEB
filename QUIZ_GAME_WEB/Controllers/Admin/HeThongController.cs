using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data; // <== SỬA TẠI ĐÂY
using QUIZ_GAME_WEB.Models.QuizModels; // DoKho, TroGiup

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class HeThongController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public HeThongController(QuizGameContext context)
        {
            _context = context;
        }

        // ===============================================
        // A. QUẢN LÝ ĐỘ KHÓ (DoKho)
        // ===============================================

        // GET: api/admin/HeThong/DoKho
        [HttpGet("DoKho")]
        public async Task<ActionResult<IEnumerable<DoKho>>> GetDoKhos()
        {
            return await _context.DoKhos.ToListAsync();
        }

        // PUT: api/admin/HeThong/DoKho/{id}
        [HttpPut("DoKho/{id}")]
        public async Task<IActionResult> PutDoKho(int id, DoKho doKhoUpdate)
        {
            if (id != doKhoUpdate.DoKhoID)
            {
                return BadRequest();
            }

            var doKho = await _context.DoKhos.FindAsync(id);
            if (doKho == null)
            {
                return NotFound();
            }

            // Logic nghiệp vụ: Chỉ cho phép cập nhật Điểm Thưởng
            doKho.DiemThuong = doKhoUpdate.DiemThuong;

            _context.Entry(doKho).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ===============================================
        // B. QUẢN LÝ TRỢ GIÚP (TroGiup)
        // ===============================================

        // GET: api/admin/HeThong/TroGiup
        [HttpGet("TroGiup")]
        public async Task<ActionResult<IEnumerable<TroGiup>>> GetTroGiups()
        {
            return await _context.TroGiups.ToListAsync();
        }

        // POST: api/admin/HeThong/TroGiup
        [HttpPost("TroGiup")]
        public async Task<ActionResult<TroGiup>> PostTroGiup(TroGiup troGiup)
        {
            _context.TroGiups.Add(troGiup);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTroGiups), new { id = troGiup.TroGiupID }, troGiup);
        }
    }
}