using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels; // Cần Comment Entity
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace QUIZ_GAME_WEB.Controllers.Social
{
    [Route("api/social/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;

        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _commentRepository = unitOfWork.Comments;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        // ===============================================
        // 1. LẤY DANH SÁCH BÌNH LUẬN (GET)
        // ===============================================
        [HttpGet("{entityType}/{entityId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(string entityType, int entityId,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // ✅ Gọi hàm Repository với 4 tham số (phân trang và ánh xạ DTO)
                var (list, total) = await _commentRepository.GetCommentsByEntityAsync(
                    entityType, entityId, pageNumber, pageSize);

                return Ok(new
                {
                    total,
                    pageSize,
                    pageNumber,
                    items = list
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy bình luận: " + ex.Message });
            }
        }

        // ===============================================
        // 2. THÊM BÌNH LUẬN MỚI (POST)
        // ===============================================
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Create([FromBody] CommentInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            var comment = new Comment
            {
                UserID = userId.Value,
                Content = model.NoiDung,
                EntityType = model.EntityType,
                RelatedEntityID = model.RelatedEntityID,
                NgayTao = DateTime.Now
                // NgayCapNhat không tồn tại và không được gán
            };

            _unitOfWork.Comments.Add(comment);
            await _unitOfWork.CompleteAsync();

            return StatusCode(201, comment);
        }

        // ===============================================
        // 3. CHỈNH SỬA BÌNH LUẬN (UPDATE)
        // ===============================================
        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Update(int id, [FromBody] CommentUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            var comment = await _unitOfWork.Comments.GetByIdAsync(id);

            if (comment == null)
                return NotFound(new { message = "Comment không tồn tại." });

            if (comment.UserID != userId.Value)
                return StatusCode(403, new { message = "Bạn không có quyền chỉnh sửa bình luận này." });

            comment.Content = model.NoiDung;
            // ✅ SỬ DỤNG LẠI NgayTao (Nếu bạn muốn cập nhật thời gian) HOẶC BỎ QUA
            // Giữ nguyên NgayTao để tránh thay đổi DB không mong muốn.

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.CompleteAsync();

            return Ok(comment);
        }

        // ===============================================
        // 4. XÓA BÌNH LUẬN (DELETE)
        // ===============================================
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            var comment = await _unitOfWork.Comments.GetByIdAsync(id);

            if (comment == null)
                return NotFound(new { message = "Comment không tồn tại." });

            if (comment.UserID != userId.Value)
            {
                return StatusCode(403, new { message = "Bạn không có quyền xoá bình luận này." });
            }

            _unitOfWork.Comments.Delete(comment);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Xoá comment thành công." });
        }
    }
}