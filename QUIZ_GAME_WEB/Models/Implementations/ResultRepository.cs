using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ResultRepository : GenericRepository<KetQua>, IResultRepository
    {
        private readonly QuizGameContext _context;

        public ResultRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // ===================== Chuỗi ngày (Streak) =====================
        public async Task<ChuoiNgay?> GetUserStreakAsync(int userId)
        {
            return await _context.ChuoiNgays.FirstOrDefaultAsync(c => c.UserID == userId);
        }

        public void AddStreak(ChuoiNgay streak)
        {
            _context.ChuoiNgays.Add(streak);
        }

        public void Update(ChuoiNgay streak)
        {
            _context.ChuoiNgays.Update(streak);
        }

        // ===================== Kết quả (KetQua) =====================
        public void AddKetQua(KetQua ketQua)
        {
            _context.KetQuas.Add(ketQua);
        }

        // ===================== Câu sai (CauSai) =====================

        /// <summary>
        /// Triển khai phương thức chính để lưu một câu trả lời sai vào DB.
        /// </summary>
        public async Task AddCauSaiAsync(CauSai cauSai)
        {
            await _context.CauSais.AddAsync(cauSai);
        }

        /// <summary>
        /// Triển khai phương thức cũ/trùng lặp AddWrongAnswerAsync để khớp IResultRepository.
        /// (Chỉ cần gọi lại AddCauSaiAsync để tránh trùng lặp logic).
        /// </summary>
        public async Task AddWrongAnswerAsync(CauSai cauSai)
        {
            await AddCauSaiAsync(cauSai);
        }

        public async Task<IEnumerable<CauSai>> GetRecentWrongAnswersAsync(int userId, int limit = 10)
        {
            return await _context.CauSais
                                 .Where(c => c.UserID == userId)
                                 .OrderByDescending(c => c.NgaySai)
                                 .Take(limit)
                                 .ToListAsync();
        }

        public async Task<int> CountWrongAnswersAsync(int userId, int attemptId)
        {
            return await _context.CauSais.CountAsync(c => c.UserID == userId && c.QuizAttemptID == attemptId);
        }

        // ===================== Thưởng hàng ngày (ThuongNgay) =====================
        public async Task<ThuongNgay?> GetDailyRewardByDateAsync(int userId, DateTime today)
        {
            return await _context.ThuongNgays.FirstOrDefaultAsync(t => t.UserID == userId && t.NgayNhan.Date == today.Date);
        }

        public void AddDailyReward(ThuongNgay newReward)
        {
            _context.ThuongNgays.Add(newReward);
        }

        // ===================== Thống kê/achievement (ThongKeNguoiDung & ThanhTuu) =====================
        public async Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today.AddDays(-30);
            DateTime end = endDate ?? DateTime.Today;

            return await _context.ThongKeNguoiDungs
                                 // Giữ nguyên logic Where để lọc theo ngày
                                 .Where(t => t.UserID == userId && t.Ngay >= start.Date && t.Ngay <= end.Date)
                                 .OrderBy(t => t.Ngay)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId)
        {
            return await _context.ThanhTuus
                                 .Where(t => t.NguoiDungID == userId)
                                 .ToListAsync();
        }
    }
}