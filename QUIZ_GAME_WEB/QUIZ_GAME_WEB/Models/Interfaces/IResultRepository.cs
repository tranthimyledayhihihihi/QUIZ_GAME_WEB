using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels; // Giả định KetQuaDto, KetQuaDetailDto tồn tại ở đây
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IResultRepository : IGenericRepository<KetQua>
    {
        // ===============================================
        // I. CÁC HÀM TRUY VẤN CƠ BẢN & STREAK
        // ===============================================

        // Chuỗi ngày (streak)
        Task<ChuoiNgay?> GetUserStreakAsync(int userId);
        void AddStreak(ChuoiNgay streak);
        void Update(ChuoiNgay streak);
        void AddKetQua(KetQua ketQua);

        // ✅ THÊM MỚI (Lỗi sai)
        Task AddCauSaiAsync(CauSai cauSai);

        // Thưởng hàng ngày
        Task<ThuongNgay?> GetDailyRewardByDateAsync(int userId, DateTime today);
        void AddDailyReward(ThuongNgay newReward);

        // Câu sai
        Task AddWrongAnswerAsync(CauSai cauSai); // (Cần loại bỏ nếu dùng AddCauSaiAsync)
        Task<IEnumerable<CauSai>> GetRecentWrongAnswersAsync(int userId, int limit = 10);
        Task<int> CountWrongAnswersAsync(int userId, int attemptId);

        // Thống kê/achievement
        Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId);

        // ===============================================
        // II. ✅ CÁC HÀM BỔ SUNG CHO LỊCH SỬ CHƠI (Controller)
        // ===============================================

        /// <summary>
        /// Lấy danh sách kết quả bài làm của người dùng (có phân trang).
        /// </summary>
        Task<(IEnumerable<KetQuaDto> Results, int TotalCount)> GetPaginatedResultsAsync(
            int userId, int pageNumber, int pageSize);

        /// <summary>
        /// Lấy chi tiết kết quả bài làm dựa trên Attempt ID (kèm kiểm tra quyền sở hữu).
        /// </summary>
        Task<KetQuaDetailDto?> GetResultDetailByAttemptIdAsync(int attemptId, int userId);
    }
}