using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Security.Claims;

[Route("api/admin/quiz-tuy-chinh")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
public class QLQuizTuyChinhController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QLQuizTuyChinhController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // =================================================
    // Helper: Lấy ID Admin hiện tại
    // =================================================
    private int GetCurrentAdminId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim?.Value, out int adminId))
            return adminId;

        throw new UnauthorizedAccessException("Không lấy được ID Admin.");
    }

    // =================================================
    // 1️⃣ DANH SÁCH QUIZ (PHÂN TRANG + LỌC TRẠNG THÁI)
    // =================================================
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var query = _unitOfWork.Quiz.GetQuizTuyChinhQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(q => q.TrangThai == status);

        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(q => q.NgayTao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new
            {
                q.QuizTuyChinhID,
                q.TenQuiz,
                q.MoTa,
                q.TrangThai,
                q.NgayTao,
                NguoiTao = q.NguoiDung.TenDangNhap
            })
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            data
        });
    }

    // =================================================
    // 2️⃣ CHI TIẾT QUIZ + DANH SÁCH CÂU HỎI
    // =================================================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var quiz = await _unitOfWork.Quiz.GetQuizTuyChinhByIdAsync(id);

        if (quiz == null)
            return NotFound(new { message = "Không tìm thấy Quiz." });

        return Ok(new
        {
            quiz.QuizTuyChinhID,
            quiz.TenQuiz,
            quiz.MoTa,
            quiz.TrangThai,
            quiz.NgayTao,
            NguoiTao = quiz.NguoiDung.TenDangNhap,
            DanhSachCauHoi = quiz.CauHois.Select(c => new
            {
                c.CauHoiID,
                c.NoiDung,
                c.TrangThaiDuyet
            })
        });
    }
    // =================================================
    // 3️⃣ DUYỆT QUIZ
    [HttpPost("{id:int}/phe-duyet")]
    public async Task<IActionResult> Approve(int id)
    {
        // Phải đảm bảo Repository lấy kèm (Include) danh sách CauHois
        var quiz = await _unitOfWork.Quiz.GetQuizTuyChinhByIdAsync(id);
        if (quiz == null) return NotFound(new { message = "Không tìm thấy Quiz." });

        int adminId = GetCurrentAdminId();

        // 1. Duyệt Bộ đề
        quiz.TrangThai = "Approved";
        quiz.AdminDuyetID = adminId;
        quiz.NgayDuyet = DateTime.UtcNow;

        // 2. Duyệt TẤT CẢ câu hỏi bên trong do người chơi này tạo
        if (quiz.CauHois != null)
        {
            foreach (var cauHoi in quiz.CauHois)
            {
                cauHoi.TrangThaiDuyet = "Approved";
                cauHoi.AdminDuyetID = adminId;
            }
        }

        _unitOfWork.Quiz.UpdateQuizTuyChinh(quiz);
        await _unitOfWork.CompleteAsync();

        return Ok(new { message = "Đã duyệt bộ đề và toàn bộ câu hỏi bên trong." });
    }

    // =================================================
    // 4️⃣ TỪ CHỐI QUIZ
    // =================================================
    [HttpPost("{id:int}/tu-choi")]
    public async Task<IActionResult> Reject(int id)
    {
        var quiz = await _unitOfWork.Quiz.GetQuizTuyChinhByIdAsync(id);
        if (quiz == null)
            return NotFound(new { message = "Không tìm thấy Quiz." });

        quiz.TrangThai = "Rejected";
        quiz.AdminDuyetID = GetCurrentAdminId();
        quiz.NgayDuyet = DateTime.UtcNow;

        _unitOfWork.Quiz.UpdateQuizTuyChinh(quiz);
        await _unitOfWork.CompleteAsync();

        return Ok(new { message = "Đã từ chối Quiz." });
    }

    // =================================================
    // 5️⃣ XÓA QUIZ (CHỈ SUPERADMIN)
    // =================================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var quiz = await _unitOfWork.Quiz.GetQuizTuyChinhByIdAsync(id);
        if (quiz == null)
            return NotFound();

        _unitOfWork.Quiz.DeleteQuizTuyChinh(quiz);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}