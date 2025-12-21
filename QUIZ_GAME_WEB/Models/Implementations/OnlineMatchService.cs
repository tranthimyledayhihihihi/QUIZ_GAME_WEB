using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

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

    private string GenerateMatchCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var rnd = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }

    // ✔ Tạo trận theo MatchCode (Private hoặc Random)
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
        await _unit.CompleteAsync(); // Lưu trận trước để có ID
                                     // === PHẦN THÊM MỚI: TẠO VÀ LƯU CÂU HỎI ===
        var randomQuestions = await _quiz.GetRandomQuestionsAsync(DEFAULT_QUESTION_COUNT, null, null);

        var matchQuestions = randomQuestions.Select((q, index) => new TranDauCauHoi
        {
            TranDauID = match.TranDauID,
            CauHoiID = q.CauHoiID,
            ThuTu = index + 1
        }).ToList();
        _unit.TranDau.AddMatchQuestions(matchQuestions);
        await _unit.CompleteAsync();
        // ==========================================
        return code;
    }

    public Task<TranDauTrucTiep?> GetMatchByCodeAsync(string matchCode)
    {
        return _unit.TranDau.GetQueryable()
            .Where(m => m.MatchCode == matchCode)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<CauHoiDisplayModel>> GetQuestionsByMatchCodeAsync(string matchCode)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) return Enumerable.Empty<CauHoiDisplayModel>();
        // SỬA: Lấy từ DB thay vì get random
        var questions = await _unit.TranDau.GetMatchQuestionsWithDetailsAsync(match.TranDauID);
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

    public async Task<bool> SubmitAnswerByMatchCodeAsync(string matchCode, int userId, MatchAnswerModel answer)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) return false;
        if (match.Player1ID != userId && match.Player2ID != userId) return false;
        // 1. Kiểm tra Đúng/Sai
        var correct = await _quiz.GetCorrectAnswerAsync(answer.CauHoiID);
        bool isCorrect = correct != null && correct.Equals(answer.DapAnDaChon, StringComparison.OrdinalIgnoreCase);
        int reward = 0;
        if (isCorrect)
        {
            double ratio = (MAX_TIME - answer.ThoiGianTraLoi) / MAX_TIME;
            reward = BASE_POINTS + (int)(BONUS_MAX * ratio);
        }
        // 2. Cộng điểm
        if (userId == match.Player1ID) match.DiemPlayer1 += reward;
        else match.DiemPlayer2 += reward;
        match.TrangThai = "DangChoi";
        _unit.TranDau.Update(match);
        // === PHẦN QUAN TRỌNG MỚI THÊM: LƯU LOG CÂU TRẢ LỜI ===
        var answerLog = new TraLoiTrucTiep
        {
            TranDauID = match.TranDauID,
            CauHoiID = answer.CauHoiID,
            UserID = userId,

            // SỬA TÊN BIẾN Ở ĐÂY:
            DapAnNguoiChoi = answer.DapAnDaChon, // Sửa từ DapAnChon
            DungHaySai = isCorrect,              // Sửa từ IsCorrect

            // SỬA KIỂU DỮ LIỆU:
            ThoiGianTraLoi = DateTime.Now,       // Lưu thời điểm hiện tại
            ThoiGianGiaiQuyet = answer.ThoiGianTraLoi, // Lưu số giây user trả lời

            DiemNhanDuoc = reward
        };
        await _unit.TranDau.AddPlayerAnswerAsync(answerLog);
        // ======================================================
        await _unit.CompleteAsync();
        return true;
    }

    public async Task<MatchResultModel> EndMatchByCodeAsync(string matchCode)
    {
        var match = await GetMatchByCodeAsync(matchCode);
        if (match == null) throw new Exception("Không tìm thấy trận.");
        // 1. Lấy danh sách câu hỏi để biết tổng số câu
        var questions = await _unit.TranDau.GetMatchQuestionsWithDetailsAsync(match.TranDauID);
        int totalQuestions = questions.Count();
        // 2. Lấy tổng số câu trả lời hiện có trong DB
        var answers = await _unit.TranDau.GetMatchAnswersAsync(match.TranDauID);
        int totalAnswers = answers.Count();
        // 3. KIỂM TRA: Nếu chưa đủ 2 người trả lời hết (Tổng câu trả lời < Tổng câu hỏi * 2)
        // Lưu ý: Logic này giả định cả 2 người phải trả lời hết. 
        // Nếu game cho phép bỏ qua câu hỏi thì logic này cần điều chỉnh đếm số câu đã nộp của từng user.
        if (totalAnswers < totalQuestions * 2)
        {
            // Trả về kết quả tạm thời là "Wait"
            return new MatchResultModel
            {
                MatchCode = matchCode,
                KetQua = "Wait", // Ký hiệu chờ
                WinnerHoTen = "Đang chờ đối thủ...",
                DiemPlayer1 = match.DiemPlayer1,
                DiemPlayer2 = match.DiemPlayer2
            };
        }
        // 4. NẾU ĐÃ ĐỦ => TÍNH TOÁN KẾT QUẢ CUỐI CÙNG (Code cũ)
        string result = "Hoa";
        int? winner = null;
        if (match.DiemPlayer1 > match.DiemPlayer2)
        {
            winner = match.Player1ID;
            result = "Thang";
        }
        else if (match.DiemPlayer2 > match.DiemPlayer1)
        {
            winner = match.Player2ID;
            result = "Thang";
        }
        match.TrangThai = "HoanThanh";
        _unit.TranDau.Update(match);
        await _unit.CompleteAsync();
        // Lấy tên người thắng
        string winnerName = "Hòa";
        if (winner.HasValue)
        {
            var user = await _unit.Users.GetByIdAsync(winner.Value);
            winnerName = user?.HoTen ?? "Unknown";
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
}
