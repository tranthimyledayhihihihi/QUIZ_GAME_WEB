using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using QUIZ_GAME_WEB.Models.ViewModels; // ✅ Cần cho RankingDto
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository<BXH> và thực thi ISocialRepository
    public class SocialRepository : GenericRepository<BXH>, ISocialRepository
    {
        private readonly QuizGameContext _context;

        public SocialRepository(QuizGameContext context) : base(context)
        {
            // Lưu ý: Nếu GenericRepository đã định nghĩa _context, nên truy cập nó qua base
            _context = context;
        }

        // ======================================================
        // I. RANKING/LEADERBOARD OPERATIONS
        // ======================================================

        /// <summary>
        /// Lấy Bảng Xếp Hạng (BXH) theo tuần hoặc tháng có phân trang.
        /// (Triển khai hàm GetLeaderboardAsync)
        /// </summary>
        public async Task<(IEnumerable<RankingDto> Ranking, int TotalCount)> GetLeaderboardAsync(
            string type, int pageNumber, int pageSize)
        {
            IQueryable<BXH> query = _context.BXHs.AsQueryable();

            // 1. Sắp xếp theo loại (Tuần/Tháng)
            if (type.Equals("weekly", StringComparison.OrdinalIgnoreCase) || type.Equals("tuan", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(b => b.DiemTuan).ThenBy(b => b.HangTuan);
            }
            else // Mặc định là Tháng (monthly/thang)
            {
                query = query.OrderByDescending(b => b.DiemThang).ThenBy(b => b.HangThang);
            }

            // 2. Đếm tổng số lượng
            var totalCount = await query.CountAsync();

            // 3. Phân trang và ánh xạ sang DTO
            var ranking = await query
                .Include(b => b.NguoiDung) // Tải thông tin người dùng
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new RankingDto
                {
                    UserID = b.UserID,
                    TenHienThi = b.NguoiDung!.HoTen ?? b.NguoiDung.TenDangNhap, // Giả định HoTen là tên hiển thị
                    AnhDaiDien = b.NguoiDung.AnhDaiDien,

                    HangTuan = b.HangTuan,
                    DiemTuan = b.DiemTuan,
                    HangThang = b.HangThang,
                    DiemThang = b.DiemThang,

                    // Giả định trạng thái Online (chỉ là giả định đơn giản, logic thực cần check bảng NguoiDungOnline)
                    IsOnline = _context.NguoiDungOnlines.Any(o => o.UserID == b.UserID && o.ThoiGianCapNhat >= DateTime.Now.AddMinutes(-5))
                })
                .AsNoTracking()
                .ToListAsync();

            return (ranking, totalCount);
        }

        // ----------------------------------------------------
        // LẤY ENTRY BXH CỦA USER (Giữ nguyên)
        // ----------------------------------------------------
        public async Task<BXH?> GetUserRankingEntryAsync(int userId)
        {
            // SỬA LỖI CS1061: Thay User bằng NguoiDung (nếu cần)
            return await _context.BXHs
                                 .Include(b => b.NguoiDung) // Để tải thông tin user
                                 .FirstOrDefaultAsync(b => b.UserID == userId);
        }

        // ======================================================
        // II. ONLINE STATUS & USER COUNT (Đã đồng bộ)
        // ======================================================

        /// <summary>
        /// Cập nhật hoặc thêm trạng thái online của người dùng.
        /// </summary>
        public async Task UpdateOrInsertOnlineStatusAsync(int userId, string status)
        {
            var onlineEntry = await _context.NguoiDungOnlines.FirstOrDefaultAsync(o => o.UserID == userId);

            if (onlineEntry == null)
            {
                await _context.NguoiDungOnlines.AddAsync(new NguoiDungOnline { UserID = userId, TrangThai = status, ThoiGianCapNhat = DateTime.Now });
            }
            else
            {
                onlineEntry.TrangThai = status;
                onlineEntry.ThoiGianCapNhat = DateTime.Now;
                _context.NguoiDungOnlines.Update(onlineEntry);
            }
        }

        /// <summary>
        /// Đếm tổng số người dùng online (Đồng bộ tên hàm ISocialRepository.CountOnlineUsersAsync)
        /// </summary>
        public async Task<int> CountOnlineUsersAsync()
        {
            var threshold = DateTime.Now.AddMinutes(-5); // Threshold 5 phút
            // Sử dụng TrangThai = 'online' (nếu có) HOẶC chỉ dựa vào ThoiGianCapNhat
            return await _context.NguoiDungOnlines.CountAsync(o => o.ThoiGianCapNhat >= threshold);
        }

        // ======================================================
        // III. SOCIAL INTERACTION (COMMENT)
        // ======================================================

        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }
    }
}