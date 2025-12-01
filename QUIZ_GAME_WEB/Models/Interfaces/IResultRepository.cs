using QUIZ_GAME_WEB.Models.ResultsModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IResultRepository : IGenericRepository<KetQua>
    {
        // Chuỗi ngày (streak)
        Task<ChuoiNgay?> GetUserStreakAsync(int userId);
        void AddStreak(ChuoiNgay streak);
        void Update(ChuoiNgay streak);
        void AddKetQua(KetQua ketQua);

        // ✅ THÊM MỚI:
        Task AddCauSaiAsync(CauSai cauSai);

        // Thưởng hàng ngày
        Task<ThuongNgay?> GetDailyRewardByDateAsync(int userId, DateTime today);
        void AddDailyReward(ThuongNgay newReward);

        // Câu sai
        Task AddWrongAnswerAsync(CauSai cauSai); // Bổ sung QuizAttemptID
        Task<IEnumerable<CauSai>> GetRecentWrongAnswersAsync(int userId, int limit = 10);

        Task<int> CountWrongAnswersAsync(int userId, int attemptId);

        // Thống kê/achievement
        Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId);
    }
}
