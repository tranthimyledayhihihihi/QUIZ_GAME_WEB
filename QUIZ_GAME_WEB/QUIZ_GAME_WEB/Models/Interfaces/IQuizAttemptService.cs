using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IQuizAttemptService
    {
        /// <summary>
        /// Bắt đầu một phiên làm bài Quiz mới
        /// </summary>
        /// <param name="userId">ID người chơi</param>
        /// <param name="options">Tùy chọn khi bắt đầu</param>
        /// <returns>ID của phiên làm bài mới</returns>
        /// 
        Task<int> StartNewQuizAttemptAsync(int userId, GameStartOptions options);
        /// <summary>
        /// Bắt đầu một phiên làm bài Quiz Ngày cố định.
        /// </summary>
        /// <param name="userId">ID người chơi.</param>
        /// <param name="cauHoiId">ID câu hỏi cố định của ngày hôm nay.</param>
        /// <returns>ID của phiên làm bài mới (attemptId).</returns>
        Task<int> StartDailyQuizAttemptAsync(int userId, int cauHoiId);

        /// <summary>
        /// Nộp đáp án cho một câu hỏi trong phiên Quiz
        /// </summary>
        /// <param name="answer">Thông tin câu hỏi và đáp án</param>
        /// <returns>True nếu lưu thành công</returns>
        Task<bool> SubmitAnswerAsync(AnswerSubmitModel answer);

        /// <summary>
        /// Kết thúc phiên làm bài và lưu kết quả vào database
        /// </summary>
        /// <param name="attemptId">ID phiên làm bài</param>
        /// <param name="userId">ID người chơi</param>
        /// <returns>Entity KetQua đã lưu</returns>
        Task<KetQua> EndAttemptAndCalculateResultAsync(int attemptId, int userId);

        /// <summary>
        /// Lấy câu hỏi tiếp theo trong phiên làm bài
        /// </summary>
        /// <param name="attemptId">ID phiên làm bài</param>
        /// <returns>Câu hỏi tiếp theo</returns>
        Task<CauHoiPlayDto?> GetNextQuestionAsync(int attemptId);

        // Dòng lỗi đã bị xóa:
        // Task EndAttemptAndCalculateResultAsync(int attemptId, object userId); 
    }
}