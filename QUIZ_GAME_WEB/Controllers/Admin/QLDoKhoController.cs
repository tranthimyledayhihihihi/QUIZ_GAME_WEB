using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.InputModels.QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.Linq;
using System.Threading.Tasks;

[Route("api/admin/dokho")]
[ApiController]
// Chỉ cho phép Admin/Moderator truy cập chức năng quản lý độ khó
[Authorize(AuthenticationSchemes = "Bearer", Roles = "SuperAdmin, Moderator")]
public class QLDoKhoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public QLDoKhoController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Hàm phụ trợ để truy cập Repository DoKho từ IQuizRepository
    // Giả định IQuizRepository có hàm GetDifficultyRepository()
    private IGenericRepository<DoKho> GetDifficultyRepository()
    {
        // Trong IQuizRepository, bạn có thể định nghĩa một hàm để trả về Repository cho DoKho
        // Tuy nhiên, vì DoKho là một Entity đơn giản, chúng ta giả định nó được truy cập trực tiếp nếu UnitOfWork cho phép,
        // hoặc được truy cập thông qua một hàm Get<T> trong QuizRepository.
        // Để đơn giản, chúng ta sẽ giả định IUnitOfWork.Quiz có thể truy cập DbSet<DoKho> qua một hàm của nó.
        // HOẶC, cách đơn giản nhất là: Cần thêm IDoKhoRepository vào IUnitOfWork và triển khai nó.
        // Giả định tạm thời: IUnitOfWork có hàm GetRepository<T>()
        // Vì chưa có IDoKhoRepository chính thức, chúng ta sẽ giả định IQuizRepository có thể truy vấn DoKho:

        // (Đây là logic giả định. Nếu bạn có IDoKhoRepository, hãy dùng _unitOfWork.Difficulties)
        return _unitOfWork.Quiz.GetDifficultyRepository();
    }


    // ===============================================
    // 1. LẤY DANH SÁCH ĐỘ KHÓ
    // ===============================================
    [HttpGet]
    public async Task<IActionResult> GetAllDifficulties()
    {
        var repo = GetDifficultyRepository();
        var difficulties = await repo.GetAllAsync();
        return Ok(difficulties);
    }

    // ===============================================
    // 2. LẤY CHI TIẾT ĐỘ KHÓ
    // ===============================================
    [HttpGet("{doKhoId:int}")]
    public async Task<IActionResult> GetDifficultyById(int doKhoId)
    {
        var repo = GetDifficultyRepository();
        var difficulty = await repo.GetByIdAsync(doKhoId);

        if (difficulty == null) return NotFound(new { message = $"Không tìm thấy Độ khó với ID: {doKhoId}." });
        return Ok(difficulty);
    }

    // ===============================================
    // 3. THÊM ĐỘ KHÓ MỚI
    // ===============================================
    // Trong QLDoKhoController.cs

    // Đảm bảo bạn có using QUIZ_GAME_WEB.Models.InputModels;
    // ...

    [HttpPost]
    public async Task<IActionResult> CreateDifficulty([FromBody] DoKhoCreateModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // 1. Ánh xạ từ DTO sang Entity DoKho (Không bao gồm ID)
        var newDifficulty = new DoKho
        {
            TenDoKho = model.TenDoKho,
            DiemThuong = model.DiemThuong,
            // DoKhoID sẽ được tạo tự động sau khi gọi CompleteAsync()
        };

        var repo = GetDifficultyRepository();

        // Kiểm tra trùng tên
        var existing = await repo.GetQueryable().AnyAsync(d => d.TenDoKho == model.TenDoKho);
        if (existing)
        {
            return BadRequest(new { message = $"Độ khó '{model.TenDoKho}' đã tồn tại." });
        }

        repo.Add(newDifficulty); // Thêm Entity mới
        await _unitOfWork.CompleteAsync(); // Lưu vào DB (ID được tạo tại đây)

        // Trả về ID mới được tạo
        return CreatedAtAction(nameof(GetDifficultyById), new { doKhoId = newDifficulty.DoKhoID }, newDifficulty);
    }

    // ===============================================
    // 4. CẬP NHẬT ĐỘ KHÓ
    // ===============================================
    // Trong QLDoKhoController.cs

    [HttpPut("{doKhoId:int}")]
    public async Task<IActionResult> UpdateDifficulty(int doKhoId, [FromBody] DoKhoCreateModel model) // ✅ Dùng CreateModel
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var repo = GetDifficultyRepository();
        var existingDifficulty = await repo.GetByIdAsync(doKhoId);

        if (existingDifficulty == null)
            return NotFound(new { message = $"Không tìm thấy Độ khó với ID: {doKhoId}." });

        // CẬP NHẬT CHỈ CÁC THUỘC TÍNH DỮ LIỆU
        existingDifficulty.TenDoKho = model.TenDoKho;
        existingDifficulty.DiemThuong = model.DiemThuong;

        repo.Update(existingDifficulty);
        await _unitOfWork.CompleteAsync();

        return Ok(new
        {
            message = $"Cập nhật Độ khó '{existingDifficulty.TenDoKho}' (ID: {doKhoId}) thành công!",
            data = existingDifficulty // Trả về đối tượng đã cập nhật
        });
    }
    // ===============================================
    // 5. XÓA ĐỘ KHÓ
    // ===============================================
    [HttpDelete("{doKhoId:int}")]
    [Authorize(Roles = "SuperAdmin")] // Chỉ SuperAdmin được xóa vĩnh viễn
    public async Task<IActionResult> DeleteDifficulty(int doKhoId)
    {
        var repo = GetDifficultyRepository();
        var difficulty = await repo.GetByIdAsync(doKhoId);

        if (difficulty == null) return NotFound(new { message = $"Không tìm thấy Độ khó với ID: {doKhoId}." });

        // ✅ Kiểm tra ràng buộc (Business Logic): Không xóa nếu có câu hỏi liên quan
        var hasRelatedQuestions = await _unitOfWork.Quiz.GetQueryable().AnyAsync(q => q.DoKhoID == doKhoId);
        if (hasRelatedQuestions)
        {
            return BadRequest(new { message = "Không thể xóa vì còn câu hỏi thuộc độ khó này. Vui lòng cập nhật câu hỏi trước." });
        }

        repo.Delete(difficulty);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}