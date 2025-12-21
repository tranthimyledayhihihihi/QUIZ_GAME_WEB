using QUIZ_GAME_WEB.Models.SocialRankingModels; // Cần cho Entity Comment
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        // ========================================================
        // I. ✅ HÀM TRUY VẤN (SỬ DỤNG ENTITY COMMENT VÀ PHÂN TRANG)
        // ========================================================

        /// <summary>
        /// Lấy danh sách bình luận (Comment) theo loại đối tượng và ID đối tượng (có phân trang).
        /// </summary>
        /// <param name="entityType">Loại đối tượng.</param>
        /// <param name="relatedEntityId">ID của đối tượng được bình luận.</param>
        /// <param name="pageNumber">Số trang.</param>
        /// <param name="pageSize">Kích thước trang.</param>
        /// <returns>Danh sách Comment Entity và tổng số bình luận (TotalCount).</returns>
        // ✅ Sửa: Trả về Comment Entity thay vì CommentDto
        Task<(IEnumerable<Comment> Comments, int TotalCount)> GetCommentsByEntityAsync(
            string entityType,
            int relatedEntityId,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Đếm tổng số bình luận cho một đối tượng cụ thể.
        /// </summary>
        Task<int> CountCommentsForEntityAsync(string entityType, int relatedEntityId);

        // ========================================================
        // II. CÁC HÀM THAO TÁC CƠ BẢN
        // ========================================================

        void Update(Comment comment);
        void Delete(Comment comment);
    }
}