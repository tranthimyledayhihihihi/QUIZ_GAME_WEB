using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels; // ✅ CẦN THIẾT cho CauHoiInfoDto
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IQuizRepository : IGenericRepository<CauHoi>
    {
        // ===============================================
        // I. CÁC HÀM TRUY VẤN CƠ BẢN
        // ===============================================
        Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId);
        Task<string?> GetCorrectAnswerAsync(int cauHoiId);
        Task<IEnumerable<ChuDe>> GetAllTopicsAsync();

        // ===============================================
        // II. CÁC HÀM THAO TÁC (CRUD/Transaction)
        // ===============================================
        void AddTopic(ChuDe topic);
        Task AddQuizTuyChinhAsync(QuizTuyChinh quiz);
        Task AddQuizAttemptAsync(QuizAttempt attempt);
        Task SaveQuizAttemptAsync(QuizAttempt attempt);

        // ===============================================
        // III. ✅ CÁC HÀM TRUY VẤN TỐI ƯU HÓA CHO API
        // ===============================================

        /// <summary>
        /// Lấy câu hỏi ngẫu nhiên và ánh xạ sang DTO (Hỗ trợ API /random).
        /// </summary>
        Task<IEnumerable<CauHoiInfoDto>> GetRandomQuestionsWithDetailsAsync(
            int count, int? chuDeId, int? doKhoId);

        /// <summary>
        /// Lọc, tìm kiếm và phân trang câu hỏi (Hỗ trợ API /search, /by-chude, /by-dokho).
        /// </summary>
        Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetQuestionsFilteredAsync(
            int pageNumber, int pageSize, string? keyword = null, int? chuDeId = null, int? doKhoId = null);

        /// <summary>
        /// Đếm tổng số câu hỏi (Hỗ trợ API /total-count).
        /// </summary>
        Task<int> CountAllCauHoisAsync();

        /// <summary>
        /// Lấy tất cả câu hỏi kèm chi tiết (Hỗ trợ API /statistics).
        /// </summary>
        Task<IEnumerable<CauHoi>> GetAllCauHoisWithDetailsAsync();

        // Giữ lại các hàm khác (nếu có)
        Task<int> CountActiveQuestionsAsync();
    }
}