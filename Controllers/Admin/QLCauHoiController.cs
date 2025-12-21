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
    // =============================================================
    // 3. TẠO CÂU HỎI MỚI (Dùng DTO để Swagger gọn hơn)
    // =============================================================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CauHoiCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Chuyển đổi từ DTO sang Entity để lưu vào DB
        var model = new CauHoi
        {
            NoiDung = dto.NoiDung,
            DapAnA = dto.DapAnA,
            DapAnB = dto.DapAnB,
            DapAnC = dto.DapAnC,
            DapAnD = dto.DapAnD,
            DapAnDung = dto.DapAnDung,
            ChuDeID = dto.ChuDeID,
            DoKhoID = dto.DoKhoID,
            HinhAnh = dto.HinhAnh,

            // Các trường hệ thống tự gán, Admin không cần nhập
            TrangThaiDuyet = "Approved",
            NgayTao = DateTime.UtcNow,
            AdminDuyetID = LayIdAdminHienTai()
        };

        _unitOfWork.Quiz.Add(model);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetCauHoi), new { id = model.CauHoiID }, model);
    }

    // =============================================================
    // 4. CẬP NHẬT CÂU HỎI
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CauHoiUpdateDto dto)
    {
        if (id != dto.CauHoiID)
            return BadRequest(new { message = "ID không khớp." });

        var existing = await _unitOfWork.Quiz.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi." });

        // Cập nhật nội dung từ DTO
        existing.NoiDung = dto.NoiDung;
        existing.DapAnA = dto.DapAnA;
        existing.DapAnB = dto.DapAnB;
        existing.DapAnC = dto.DapAnC;
        existing.DapAnD = dto.DapAnD;
        existing.DapAnDung = dto.DapAnDung;
        existing.ChuDeID = dto.ChuDeID;
        existing.DoKhoID = dto.DoKhoID;
        existing.HinhAnh = dto.HinhAnh;

        // Tự động gán trạng thái Approved và ghi nhận Admin thực hiện
        existing.TrangThaiDuyet = "Approved";
        existing.AdminDuyetID = LayIdAdminHienTai();

        _unitOfWork.Quiz.Update(existing);
        await _unitOfWork.CompleteAsync();

        return Ok(new { message = "Cập nhật câu hỏi thành công!" });
    }

    // =============================================================
    // 5. XÓA CÂU HỎI
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