// Models/Interfaces/ITranDauRepository.cs
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels; // Cần cho TraLoiTrucTiep
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Kế thừa GenericRepository, sử dụng Entity TranDauTrucTiep
    public interface ITranDauRepository : IGenericRepository<TranDauTrucTiep>
    {
        // === LOGIC TRẬN ĐẤU ===

        // 1. Thao tác Log
        Task<IEnumerable<CauHoi>> GetMatchQuestionsWithDetailsAsync(int tranDauId);
        // Trong Models/Interfaces/ITranDauRepository.cs
        // (Bổ sung)
        // Trong Models/Interfaces/ITranDauRepository.cs (Bổ sung)

        Task<IEnumerable<TraLoiTrucTiep>> GetAllAnswersForMatchAsync(int tranDauId);

        Task AddPlayerAnswerAsync(TraLoiTrucTiep answerLog);
        Task<IEnumerable<TraLoiTrucTiep>> GetMatchAnswersAsync(int tranDauId);

        // 2. Quản lý câu hỏi trong trận
        void AddMatchQuestions(IEnumerable<TranDauCauHoi> matchQuestions);
        Task<IEnumerable<CauHoi>> GetQuestionsByMatchIdAsync(int tranDauId);

        // 3. Truy vấn trạng thái
        Task<TranDauTrucTiep?> GetMatchStatusAsync(int tranDauId);

        // 4. Cập nhật kết quả cuối cùng
        Task UpdateMatchResultAsync(int tranDauId, int? winnerId, int diemPlayer1, int diemPlayer2);
    }
}