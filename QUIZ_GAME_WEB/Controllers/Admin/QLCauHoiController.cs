using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Security.Claims;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
public class QLCauHoiController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QLCauHoiController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private int LayIdAdminHienTai()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim?.Value, out int userId)) return userId;
        throw new UnauthorizedAccessException("Không lấy được ID người dùng.");
    }

    // =============================================================
    // 1. LẤY DANH SÁCH CÂU HỎI (PHÂN TRANG + LỌC + TRẢ VỀ DTO)
    // =============================================================
    [HttpGet]
    public async Task<IActionResult> GetQuestions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        Console.WriteLine($"=== ENDPOINT: GetQuestions (page={page}, pageSize={pageSize}, status={status}) ===");

        var (questions, totalCount) = await _unitOfWork.Quiz.GetQuestionsFilteredAsync(
            page, pageSize, null, null, null
        );

        // ✅ Filter trên DTO (Repository đã trả về DTO rồi)
        if (!string.IsNullOrWhiteSpace(status))
            questions = questions.Where(q => q.TrangThaiDuyet == status);

        Console.WriteLine($"Returning {questions.Count()} questions, Type: {questions.FirstOrDefault()?.GetType().Name}");

        return Ok(new
        {
            total = totalCount,
            page,
            pageSize,
            data = questions  // ✅ Trả về DTO trực tiếp
        });
    }

    // =============================================================
    // 2. LẤY CHI TIẾT CÂU HỎI (DTO)
    // =============================================================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCauHoi(int id)
    {
        Console.WriteLine($"=== ENDPOINT: GetCauHoi (id={id}) ===");

        var cauHoi = await _unitOfWork.Quiz.GetByIdAsync(id);
        if (cauHoi == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi." });

        Console.WriteLine($"Entity type: {cauHoi.GetType().Name}");
        Console.WriteLine($"Has ChuDe? {cauHoi.ChuDe != null}");
        Console.WriteLine($"Has DoKho? {cauHoi.DoKho != null}");

        var dto = new CauHoiInfoDto
        {
            CauHoiID = cauHoi.CauHoiID,
            NoiDung = cauHoi.NoiDung,
            DapAnA = cauHoi.DapAnA,
            DapAnB = cauHoi.DapAnB,
            DapAnC = cauHoi.DapAnC,
            DapAnD = cauHoi.DapAnD,
            HinhAnh = cauHoi.HinhAnh,
            ChuDeID = cauHoi.ChuDeID,
            DoKhoID = cauHoi.DoKhoID,
            TrangThaiDuyet = cauHoi.TrangThaiDuyet,
            TenChuDe = string.Empty,
            TenDoKho = string.Empty,
            DiemThuong = 0
        };

        Console.WriteLine("Returning DTO");
        return Ok(dto);
    }

    // =============================================================
    // 3. TẠO CÂU HỎI MỚI
    // =============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CauHoi model)
    {
        Console.WriteLine("=== ENDPOINT: Create ===");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        model.TrangThaiDuyet = "Approved"; // Admin tạo = tự duyệt
        model.NgayTao = DateTime.UtcNow;

        _unitOfWork.Quiz.Add(model);
        await _unitOfWork.CompleteAsync();

        Console.WriteLine($"Created question ID: {model.CauHoiID}");

        return CreatedAtAction(nameof(GetCauHoi), new { id = model.CauHoiID }, new
        {
            model.CauHoiID,
            model.NoiDung,
            model.ChuDeID,
            model.DoKhoID
        });
    }

    // =============================================================
    // 4. CẬP NHẬT CÂU HỎI
    // =============================================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CauHoi updated)
    {
        Console.WriteLine($"=== ENDPOINT: Update (id={id}) ===");

        if (id != updated.CauHoiID)
            return BadRequest(new { message = "ID không khớp." });

        var existing = await _unitOfWork.Quiz.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi." });

        existing.NoiDung = updated.NoiDung;
        existing.DapAnA = updated.DapAnA;
        existing.DapAnB = updated.DapAnB;
        existing.DapAnC = updated.DapAnC;
        existing.DapAnD = updated.DapAnD;
        existing.DapAnDung = updated.DapAnDung;
        existing.ChuDeID = updated.ChuDeID;
        existing.DoKhoID = updated.DoKhoID;
        existing.HinhAnh = updated.HinhAnh;

        _unitOfWork.Quiz.Update(existing);
        await _unitOfWork.CompleteAsync();

        Console.WriteLine("Update completed");
        return NoContent();
    }

    // =============================================================
    // 5. PHÊ DUYỆT CÂU HỎI UGC
    // =============================================================
    [HttpPost("phe-duyet/{id:int}")]
    public async Task<IActionResult> Approve(int id)
    {
        Console.WriteLine($"=== ENDPOINT: Approve (id={id}) ===");

        var adminId = LayIdAdminHienTai();
        var cauHoi = await _unitOfWork.Quiz.GetByIdAsync(id);

        if (cauHoi == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi." });

        cauHoi.TrangThaiDuyet = "Approved";
        cauHoi.AdminDuyetID = adminId;

        _unitOfWork.Quiz.Update(cauHoi);
        await _unitOfWork.CompleteAsync();

        Console.WriteLine("Question approved");
        return Ok(new { message = "Đã duyệt câu hỏi." });
    }

    // =============================================================
    // 6. TỪ CHỐI CÂU HỎI UGC
    // =============================================================
    [HttpPost("tu-choi/{id:int}")]
    public async Task<IActionResult> Reject(int id)
    {
        Console.WriteLine($"=== ENDPOINT: Reject (id={id}) ===");

        var adminId = LayIdAdminHienTai();
        var cauHoi = await _unitOfWork.Quiz.GetByIdAsync(id);

        if (cauHoi == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi." });

        cauHoi.TrangThaiDuyet = "Rejected";
        cauHoi.AdminDuyetID = adminId;

        _unitOfWork.Quiz.Update(cauHoi);
        await _unitOfWork.CompleteAsync();

        Console.WriteLine("Question rejected");
        return Ok(new { message = "Đã từ chối câu hỏi." });
    }

    // =============================================================
    // 7. XÓA CÂU HỎI
    // =============================================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        Console.WriteLine($"=== ENDPOINT: Delete (id={id}) ===");

        var cauHoi = await _unitOfWork.Quiz.GetByIdAsync(id);
        if (cauHoi == null)
            return NotFound();

        _unitOfWork.Quiz.Delete(cauHoi);
        await _unitOfWork.CompleteAsync();

        Console.WriteLine("Question deleted");
        return NoContent();
    }
}