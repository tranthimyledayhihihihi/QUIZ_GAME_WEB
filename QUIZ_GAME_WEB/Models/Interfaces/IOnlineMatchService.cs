using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOnlineMatchService
{
    // ===============================
    // MATCH CORE
    // ===============================
    Task<string> CreateMatchAsync(int player1Id, int? player2Id = null);
    Task<TranDauTrucTiep?> GetMatchByCodeAsync(string matchCode);
    Task UpdateMatchAsync(TranDauTrucTiep match);

    // ✅ THÊM 2 METHODS MỚI
    Task<string> CreateMatchWithBothPlayersAsync(int player1Id, int player2Id);
    Task DeleteMatchAsync(string matchCode);

    // ===============================
    // GAMEPLAY
    // ===============================
    Task<IEnumerable<CauHoiDisplayModel>> GetQuestionsByMatchCodeAsync(string matchCode);
    Task<bool> SubmitAnswerByMatchCodeAsync(
        string matchCode,
        int userId,
        MatchAnswerModel answer
    );
    Task<MatchResultModel> EndMatchByCodeAsync(string matchCode);
    Task<IEnumerable<TranDauTrucTiep>> GetMatchHistoryAsync(int userId);

    // ✅ THÊM METHOD MỚI - ĐẾM SỐ CÂU TRẢ LỜI
    Task<int> GetMatchAnswersCountAsync(string matchCode);
}