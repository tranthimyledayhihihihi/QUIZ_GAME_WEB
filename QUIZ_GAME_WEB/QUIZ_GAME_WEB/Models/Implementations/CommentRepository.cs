using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly QuizGameContext _context;

        public CommentRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // ===============================================
        // I. HÀM PHÂN TRANG (KHẮC PHỤC LỖI RETURN TYPE)
        // ===============================================

        public async Task<(IEnumerable<Comment> Comments, int TotalCount)> GetCommentsByEntityAsync(
            string entityType,
            int relatedEntityId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Comments
                .Where(c => c.EntityType == entityType && c.RelatedEntityID == relatedEntityId)
                .OrderByDescending(c => c.NgayTao)
                .AsQueryable();

            // 1. Tải thông tin người dùng (cho hiển thị)
            query = query.Include(c => c.User).AsNoTracking();

            // 2. Đếm tổng số lượng
            var totalCount = await query.CountAsync();

            // 3. Phân trang và Lấy Entity
            var comments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // ✅ TRẢ VỀ CHÍNH XÁC LIST<Comment> ENTITY

            // ✅ TRẢ VỀ CHÍNH XÁC KIỂU TUPLE ĐƯỢC YÊU CẦU TRONG INTERFACE
            return (comments, totalCount);
        }

        /// <summary>
        /// Đếm tổng số bình luận cho một đối tượng cụ thể.
        /// </summary>
        public async Task<int> CountCommentsForEntityAsync(string entityType, int relatedEntityId)
        {
            return await _context.Comments
                .CountAsync(c => c.EntityType == entityType && c.RelatedEntityID == relatedEntityId);
        }

        // ===============================================
        // II. HÀM THAO TÁC CƠ BẢN
        // ===============================================

        public void Update(Comment comment)
        {
            _context.Comments.Update(comment);
        }

        public void Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        // ===============================================
        // III. HÀM CŨ (Giữ lại nếu cần, nhưng không phải là triển khai Interface)
        // ===============================================

        public async Task<IEnumerable<Comment>> GetCommentsByEntityAsync(string entityType, int relatedEntityId)
        {
            return await _context.Comments
                .Where(c => c.EntityType == entityType && c.RelatedEntityID == relatedEntityId)
                .OrderByDescending(c => c.NgayTao)
                .ToListAsync();
        }
    }
}