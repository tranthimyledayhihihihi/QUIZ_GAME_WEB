using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class OnlineMatchService : IOnlineMatchService
{
    private readonly IUnitOfWork _unit;
    private readonly IQuizRepository _quiz;

    private const int DEFAULT_QUESTION_COUNT = 10;
    private const int BASE_POINTS = 100;
    private const double MAX_TIME = 15.0;
    private const int BONUS_MAX = 50;

    public OnlineMatchService(IUnitOfWork unit, IQuizRepository quiz)
    {
        _unit = unit;
        _quiz = quiz;
    }

    // =====================================================
    // UTIL
    // =====================================================
    private string GenerateMatchCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var rnd = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }

    // =====================================================
    // CREATE MATCH
    // =====================================================
    public async Task<string> CreateMatchAsync(int player1Id, int? player2Id = null)
    {
        string code = GenerateMatchCode();

        var match = new TranDauTrucTiep
        {
            MatchCode = code,
            Player1ID = player1Id,
            Player2ID = player2Id ?? 0,
            TrangThai = player2Id == null ? "ChoNguoiChoi" : "DangChoi",
            ThoiGianBatDau = DateTime.Now
        };

        _unit.TranDau.Add(match);
        await _unit.CompleteAsync();

        // tạo danh sách câu hỏi cố định cho trận
        var randomQuestions =
            await _quiz.GetRandomQuestionsAsync(DEFAULT_QUESTION_COUNT, null, null);

        var matchQuestions = randomQuestions.Select((q, index) => new TranDauCauHoi
        {
            TranDauID = match.TranDauID,
            CauHoiID = q.CauHoiID,
            ThuTu = index + 1
        }).ToList();

        _unit.TranDau.AddMatchQuestions(matchQuestions);
        await _unit.CompleteAsync();

        return code;
    }

    // =====================================================
    // GET MATCH
    // =====================================================
    public Task<TranDauTrucTiep?> GetMatchByCodeAsync(string matchCode)
    {
        return _unit.TranDau.GetQueryable()
            .Where(m => m.MatchCode == matchCode)
            .FirstOrDefaultAsync();
    }

    // =====================================================
    // UPDATE MATCH (🔥 THÊM – FIX CONTROLLER)
    // =====================================================
    public async Task UpdateMatchAsync(TranDauTrucTiep match)
    {
        _unit.TranDau.Update(match);
        await _unit.CompleteAsync();
    }

    // =====================================================
    // GET QUESTIONS
    // =====================================================
    public async Task<IEnumerable<CauHoiDisplayModel>> GetQuestionsByMatchCodeAsync(string matchCode)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) return Enumerable.Empty<CauHoiDisplayModel>();

        var questions =
            await _unit.TranDau.GetMatchQuestionsWithDetailsAsync(match.TranDauID);

        return questions.Select((q, i) => new CauHoiDisplayModel
        {
            CauHoiID = q.CauHoiID,
            NoiDung = q.NoiDung,
            CacLuaChon = JsonSerializer.Serialize(new
            {
                A = q.DapAnA,
                B = q.DapAnB,
                C = q.DapAnC,
                D = q.DapAnD
            }),
            ThuTuTrongTranDau = i + 1,
            ThoiGianToiDa = MAX_TIME
        });
    }
    // =====================================================
    // SUBMIT ANSWER
    // =====================================================
    public async Task<bool> SubmitAnswerByMatchCodeAsync(
        string matchCode,
        int userId,
        MatchAnswerModel answer)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) return false;
        if (match.Player1ID != userId && match.Player2ID != userId) return false;

        var correct = await _quiz.GetCorrectAnswerAsync(answer.CauHoiID);
        bool isCorrect =
            correct != null &&
            correct.Equals(answer.DapAnDaChon, StringComparison.OrdinalIgnoreCase);

        // ✅ SỬA: Chỉ cộng điểm khi đúng, không cộng khi sai
        int reward = 0;
        if (isCorrect)
        {
            reward = BASE_POINTS; // ✅ 100 điểm cố định cho mỗi câu đúng
        }

        // ✅ Cộng điểm vào người chơi
        if (userId == match.Player1ID)
            match.DiemPlayer1 += reward;
        else
            match.DiemPlayer2 += reward;

        match.TrangThai = "DangChoi";
        _unit.TranDau.Update(match);

        var answerLog = new TraLoiTrucTiep
        {
            TranDauID = match.TranDauID,
            CauHoiID = answer.CauHoiID,
            UserID = userId,
            DapAnNguoiChoi = answer.DapAnDaChon,
            DungHaySai = isCorrect,
            ThoiGianTraLoi = DateTime.Now,
            ThoiGianGiaiQuyet = answer.ThoiGianTraLoi,
            DiemNhanDuoc = reward
        };

        await _unit.TranDau.AddPlayerAnswerAsync(answerLog);
        await _unit.CompleteAsync();

        return isCorrect; // ✅ Trả về true nếu đúng, false nếu sai
    }
    // =====================================================
    // END MATCH
    // =====================================================
    public async Task<MatchResultModel> EndMatchByCodeAsync(string matchCode)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) throw new Exception("Không tìm thấy trận.");

        // 1. Lấy danh sách câu hỏi và câu trả lời thực tế trong DB
        var questions = await _unit.TranDau.GetMatchQuestionsWithDetailsAsync(match.TranDauID);
        var answers = await _unit.TranDau.GetMatchAnswersAsync(match.TranDauID);

        int totalQuestions = questions.Count();
        int totalAnswersNeeded = totalQuestions * 2; // Mỗi người phải trả lời hết số câu hỏi

        // 2. KIỂM TRA: Nếu chưa đủ số câu trả lời từ cả 2 phía
        if (answers.Count() < totalAnswersNeeded)
        {
            return new MatchResultModel
            {
                MatchCode = matchCode,
                KetQua = "Wait", // Ký hiệu chưa xong để Server không gửi GAME_END
                WinnerHoTen = "Đang chờ đối thủ...",
                DiemPlayer1 = match.DiemPlayer1,
                DiemPlayer2 = match.DiemPlayer2
            };
        }

        // 3. NẾU ĐÃ ĐỦ => TÍNH TOÁN KẾT QUẢ CUỐI CÙNG
        string result = "Hoa";
        int? winnerId = null;

        if (match.DiemPlayer1 > match.DiemPlayer2)
        {
            winnerId = match.Player1ID;
            result = "Thang";
        }
        else if (match.DiemPlayer2 > match.DiemPlayer1)
        {
            winnerId = match.Player2ID;
            result = "Thang";
        }

        // Cập nhật trạng thái trận đấu
        match.TrangThai = "HoanThanh";
        _unit.TranDau.Update(match);
        await _unit.CompleteAsync();

        // Lấy tên người thắng để hiển thị
        string winnerName = "Hòa";
        if (winnerId.HasValue)
        {
            var user = await _unit.Users.GetByIdAsync(winnerId.Value);
            winnerName = user?.HoTen ?? "Người chơi";
        }

        return new MatchResultModel
        {
            MatchCode = matchCode,
            KetQua = result,
            WinnerHoTen = winnerName,
            DiemPlayer1 = match.DiemPlayer1,
            DiemPlayer2 = match.DiemPlayer2
        };
    }
    // =====================================================
    public async Task<string> CreateMatchWithBothPlayersAsync(int player1Id, int player2Id)
    {
        try
        {
            Console.WriteLine($"[SERVICE] 📝 Creating match for Player {player1Id} vs Player {player2Id}");

            string code = GenerateMatchCode();

            var match = new TranDauTrucTiep
            {
                MatchCode = code,
                Player1ID = player1Id,
                Player2ID = player2Id,
                TrangThai = "DangChoi",
                ThoiGianBatDau = DateTime.Now,
                DiemPlayer1 = 0,
                DiemPlayer2 = 0
            };

            _unit.TranDau.Add(match);
            await _unit.CompleteAsync();

            Console.WriteLine($"[SERVICE] ✅ Match {code} added to DB");

            // ✅ Verify match đã được tạo
            var createdMatch = await _unit.TranDau.GetQueryable()
                .FirstOrDefaultAsync(m => m.MatchCode == code);

            if (createdMatch == null)
            {
                Console.WriteLine($"[SERVICE] ❌ Match verification failed!");
                return null;
            }

            Console.WriteLine($"[SERVICE] ✅ Match verified: TranDauID = {createdMatch.TranDauID}");

            // ✅ Tạo danh sách câu hỏi
            var randomQuestions = await _quiz.GetRandomQuestionsAsync(DEFAULT_QUESTION_COUNT, null, null);

            var matchQuestions = randomQuestions.Select((q, index) => new TranDauCauHoi
            {
                TranDauID = createdMatch.TranDauID,
                CauHoiID = q.CauHoiID,
                ThuTu = index + 1
            }).ToList();

            _unit.TranDau.AddMatchQuestions(matchQuestions);
            await _unit.CompleteAsync();

            Console.WriteLine($"[SERVICE] ✅ Added {matchQuestions.Count} questions to match {code}");

            return code;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVICE] ❌ Error creating match: {ex.Message}");
            Console.WriteLine($"[SERVICE] ❌ Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    // =====================================================
    // ✅ THÊM MỚI: DELETE MATCH
    // =====================================================
    public async Task DeleteMatchAsync(string matchCode)
    {
        try
        {
            var match = await _unit.TranDau.GetQueryable()
                .FirstOrDefaultAsync(m => m.MatchCode == matchCode);

            if (match != null)
            {
                Console.WriteLine($"[SERVICE] 🗑️ Deleting match {matchCode}...");

                // Xóa câu hỏi trước
                var questions = _unit.TranDau.GetQueryable()
                    .Where(q => q.TranDauID == match.TranDauID);

                // Xóa match
                _unit.TranDau.Delete(match);
                await _unit.CompleteAsync();

                Console.WriteLine($"[SERVICE] ✅ Match {matchCode} deleted successfully");
            }
            else
            {
                Console.WriteLine($"[SERVICE] ⚠️ Match {matchCode} not found for deletion");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVICE] ❌ Error deleting match: {ex.Message}");
        }
    }
    // =====================================================
    // MATCH HISTORY (🔥 THÊM – FIX CONTROLLER)
    // =====================================================
    public async Task<IEnumerable<TranDauTrucTiep>> GetMatchHistoryAsync(int userId)
    {
        return await _unit.TranDau.GetQueryable()
            .Where(t => t.Player1ID == userId || t.Player2ID == userId)
            .OrderByDescending(t => t.ThoiGianBatDau)
            .ToListAsync();
    }
    // =====================================================
    // ✅ THÊM MỚI: GET MATCH ANSWERS COUNT
    // =====================================================
    public async Task<int> GetMatchAnswersCountAsync(string matchCode)
    {
        try
        {
            var match = await GetMatchByCodeAsync(matchCode);
            if (match == null)
            {
                Console.WriteLine($"[SERVICE] ⚠️ Match {matchCode} not found for answer count");
                return 0;
            }

            var answers = await _unit.TranDau.GetMatchAnswersAsync(match.TranDauID);
            int count = answers.Count();

            Console.WriteLine($"[SERVICE] 📊 Match {matchCode} has {count} answers");
            return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SERVICE] ❌ Error getting answer count: {ex.Message}");
            return 0;
        }
    }
    public async Task<int> GetPlayerAnswersCountAsync(string matchCode, int userId)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) return 0;

        var answers = await _unit.TranDau.GetMatchAnswersAsync(match.TranDauID);
        return answers.Count(a => a.UserID == userId);
    }

   

}