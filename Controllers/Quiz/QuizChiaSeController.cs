using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels; // Cần QuizChiaSe
using QUIZ_GAME_WEB.Models.ViewModels; // Cần DTOs (QuizShareDto, QuizShareDetailDto)
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Bắt buộc đăng nhập
    public class QuizChiaSeController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuizChiaSeController(IQuizRepository quizRepository, IUnitOfWork unitOfWork)
        {
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
        }

        private int GetUserIdFromClaim()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID không hợp lệ trong token.");
            }
            return userId;
        }

        // DTO nhỏ cho request share (Giả định nằm trong Models/InputModels)
        public class ShareQuizRequest
        {
            public int QuizTuyChinhID { get; set; }
            public int UserNhanID { get; set; }
            // Có thể thêm message, hạn dùng, v.v.
        }

        // ======================================================
        // 1. SHARE QUIZ TUỲ CHỈNH CHO NGƯỜI KHÁC (POST: api/QuizChiaSe/share)
        // ======================================================
        [HttpPost("share")]
        public async Task<IActionResult> ShareQuiz([FromBody] ShareQuizRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                int userGuiId = GetUserIdFromClaim();

                if (userGuiId == request.UserNhanID)
                {
                    return BadRequest(new { message = "Không thể chia sẻ Quiz cho chính mình." });
                }

                // ✅ Bước 1: Kiểm tra Quiz tồn tại và thuộc sở hữu của người gửi
                bool canShare = await _quizRepository.CheckQuizOwnershipAndExistenceAsync(
                    request.QuizTuyChinhID, userGuiId);

                if (!canShare)
                {
                    return NotFound(new { message = "Quiz không tồn tại hoặc bạn không có quyền chia sẻ." });
                }

                // ✅ Bước 2: Kiểm tra người nhận có tồn tại không
                var receiverExists = await _unitOfWork.Users.GetByIdAsync(request.UserNhanID);
                if (receiverExists == null)
                {
                    return NotFound(new { message = "Người dùng nhận không tồn tại." });
                }

                // ✅ Bước 3: Tạo entity QuizChiaSe
                var share = new Models.QuizModels.QuizChiaSe
                {
                    QuizTuyChinhID = request.QuizTuyChinhID,
                    UserGuiID = userGuiId,
                    UserNhanID = request.UserNhanID,
                    NgayChiaSe = DateTime.Now
                };

                // ✅ Bước 4: Lưu vào DB
                await _quizRepository.AddQuizChiaSeAsync(share);
                await _unitOfWork.CompleteAsync();

                return Ok(new { message = $"Chia sẻ Quiz {request.QuizTuyChinhID} thành công cho người dùng {request.UserNhanID}." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi chia sẻ quiz: " + ex.Message });
            }
        }

        // ======================================================
        // 2. DANH SÁCH QUIZ MÌNH ĐÃ GỬI (GET: api/QuizChiaSe/sent)
        // ======================================================
        [HttpGet("sent")]
        public async Task<IActionResult> GetSentQuizShares()
        {
            try
            {
                int userId = GetUserIdFromClaim();

                var (shares, totalCount) = await _quizRepository.GetSharedQuizzesBySenderAsync(userId);

                return Ok(new { tongSoGui = totalCount, danhSach = shares });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy danh sách quiz đã chia sẻ: " + ex.Message });
            }
        }

        // ======================================================
        // 3. DANH SÁCH QUIZ MÌNH ĐƯỢC NHẬN (GET: api/QuizChiaSe/received)
        // ======================================================
        [HttpGet("received")]
        public async Task<IActionResult> GetReceivedQuizShares()
        {
            try
            {
                int userId = GetUserIdFromClaim();

                var (shares, totalCount) = await _quizRepository.GetSharedQuizzesByReceiverAsync(userId);

                return Ok(new { tongSoNhan = totalCount, danhSach = shares });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy danh sách quiz nhận được: " + ex.Message });
            }
        }

        // ======================================================
        // 4. CHI TIẾT MỘT LẦN SHARE (GET: api/QuizChiaSe/{id})
        // ======================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetShareDetail(int id)
        {
            try
            {
                int userId = GetUserIdFromClaim();

                var shareDetail = await _quizRepository.GetShareDetailByIdAsync(id);

                if (shareDetail == null)
                {
                    return NotFound(new { message = "Không tìm thấy bản ghi chia sẻ." });
                }

                // ✅ Kiểm tra quyền truy cập: Người dùng phải là người gửi HOẶC người nhận
                if (shareDetail.UserGuiID != userId && shareDetail.UserNhanID != userId)
                {
                    return StatusCode(403, new { message = "Bạn không có quyền xem chi tiết chia sẻ này." });
                }

                return Ok(shareDetail); // Trả về QuizShareDetailDto
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy chi tiết chia sẻ: " + ex.Message });
            }
        }
    }
}