using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.QuizModels; // ChuDe

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class QLChuDeController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QLChuDeController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/admin/QLChuDe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChuDe>>> GetChuDes()
        {
            return await _context.ChuDes.ToListAsync();
        }

        // POST: api/admin/QLChuDe
        [HttpPost]
        public async Task<ActionResult<ChuDe>> PostChuDe(ChuDe chuDe)
        {
            // Logic nghiệp vụ: Kiểm tra trùng tên
            if (_context.ChuDes.Any(c => c.TenChuDe == chuDe.TenChuDe))
            {
                return Conflict("Tên Chủ Đề đã tồn tại.");
            }

            _context.ChuDes.Add(chuDe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChuDes), new { id = chuDe.ChuDeID }, chuDe);
        }

        // PUT: api/admin/QLChuDe/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChuDe(int id, ChuDe chuDe)
        {
            if (id != chuDe.ChuDeID)
            {
                return BadRequest();
            }

            // Logic nghiệp vụ: Kiểm tra trùng tên (trừ chính nó)
            if (_context.ChuDes.Any(c => c.TenChuDe == chuDe.TenChuDe && c.ChuDeID != id))
            {
                return Conflict("Tên Chủ Đề đã được sử dụng bởi chủ đề khác.");
            }

            _context.Entry(chuDe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ChuDes.Any(e => e.ChuDeID == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/admin/QLChuDe/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChuDe(int id)
        {
            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }

            // Logic nghiệp vụ: Ngăn xóa nếu Chủ đề đang được sử dụng
            if (await _context.CauHois.AnyAsync(ch => ch.ChuDeID == id))
            {
                return Conflict("Không thể xóa. Chủ đề này đang được sử dụng trong các câu hỏi.");
            }

            _context.ChuDes.Remove(chuDe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}