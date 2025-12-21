using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using QUIZ_GAME_WEB.Models.InputModels; // ✅ CẦN THIẾT
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IQuizRepository : IGenericRepository<CauHoi>
    {
        // ... (I. CÁC HÀM TRUY VẤN CƠ BẢN) ...
        Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId);
        Task<string?> GetCorrectAnswerAsync(int cauHoiId);
        Task<IEnumerable<ChuDe>> GetAllTopicsAsync();

        // ... (II. CÁC HÀM THAO TÁC) ...
        void AddTopic(ChuDe topic);
        Task AddQuizTuyChinhAsync(QuizTuyChinh quiz);
        Task AddQuizAttemptAsync(QuizAttempt attempt);
        Task SaveQuizAttemptAsync(QuizAttempt attempt);

        // ... (III. CÁC HÀM TRUY VẤN TỐI ƯU HÓA CHO API) ...
        Task<IEnumerable<CauHoiInfoDto>> GetRandomQuestionsWithDetailsAsync(int count, int? chuDeId, int? doKhoId);
        Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetQuestionsFilteredAsync(
            int pageNumber, int pageSize, string? keyword = null, int? chuDeId = null, int? doKhoId = null);
        Task<int> CountAllCauHoisAsync();
        Task<IEnumerable<CauHoi>> GetAllCauHoisWithDetailsAsync();
        Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetIncorrectQuestionsByUserIdAsync(
            int userId, int pageNumber, int pageSize);
        Task<int> CountActiveQuestionsAsync();
        Task<IQueryable<CauHoi>> GetPendingQuestionsAsync();
        Task<bool> ApproveQuestionAsync(int cauHoiId, int adminId);

        // ✅ BỔ SUNG: Cho phép truy cập Repository của các Entity liên quan

        /// <summary>
        /// Trả về Generic Repository để quản lý các Entity ChuDe (Topic).
        /// </summary>
        IGenericRepository<ChuDe> GetTopicRepository();

        /// <summary>
        /// Trả về Generic Repository để quản lý các Entity DoKho (Difficulty).
        /// </summary>
        IGenericRepository<DoKho> GetDifficultyRepository();

        // ===============================================
        // IV. ✅ CÁC HÀM QUẢN LÝ QUIZ TÙY CHỈNH (UGC)
        // ===============================================
        /// Thêm bản ghi QuizChiaSe mới vào DB.
        /// </summary>
        Task AddQuizChiaSeAsync(QuizChiaSe share);

        /// <summary>
        /// Kiểm tra xem Quiz có tồn tại và thuộc sở hữu của người dùng hay không.
        /// (Dùng để xác thực trước khi Share)
        /// </summary>
        Task<bool> CheckQuizOwnershipAndExistenceAsync(int quizId, int userId);

        // ===============================================
        // II. ✅ CÁC HÀM TRUY VẤN LỊCH SỬ CHIA SẺ
        // ===============================================
        Task<CauHoiInfoDto?> GetQuestionDetailByIdAsync(int cauHoiId);
        /// <summary>
        /// Lấy danh sách các Quiz mà người dùng đã gửi đi.
        /// </summary>
        Task<(IEnumerable<QuizShareDto> Shares, int TotalCount)> GetSharedQuizzesBySenderAsync(int userId);

        /// <summary>
        /// Lấy danh sách các Quiz mà người dùng đã nhận được.
        /// </summary>
        Task<(IEnumerable<QuizShareDto> Shares, int TotalCount)> GetSharedQuizzesByReceiverAsync(int userId);

        /// <summary>
        /// Lấy chi tiết một bản ghi chia sẻ cụ thể.
        /// </summary>
        Task<QuizNgayDetailsDto?> GetTodayQuizDetailsAsync();
        Task<QuizShareDetailDto?> GetShareDetailByIdAsync(int shareId);

        Task<(IEnumerable<QuizTuyChinhDto> Quizzes, int TotalCount)> GetQuizSubmissionsByUserIdAsync(
            int userId, int pageNumber, int pageSize);

        Task<QuizTuyChinh?> GetQuizSubmissionByIdAsync(int quizId);

        Task<QuizTuyChinh> SubmitNewQuizAsync(int userId, QuizSubmissionModel submission);

        Task<bool> DeleteQuizSubmissionAsync(int quizId, int userId);
    }
}