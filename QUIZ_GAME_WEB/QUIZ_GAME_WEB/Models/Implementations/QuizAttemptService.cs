using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class QuizAttemptService : IQuizAttemptService
    {
        private readonly IUnitOfWork _unitOfWork;

        // BỎ ID TẠM THỜI: ID sẽ được DB tự động tạo
        private static readonly Dictionary<int, QuizSessionData> _activeSessions = new();
        // private static int _attemptIdCounter = 1000; // ĐÃ BỎ

        public QuizAttemptService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<int> StartDailyQuizAttemptAsync(int userId, int cauHoiId)
        {
            // 1. Lấy câu hỏi cố định của ngày hôm nay
            var question = await _unitOfWork.Quiz.GetByIdAsync(cauHoiId);

            if (question == null)
            {
                throw new Exception($"Không tìm thấy câu hỏi với ID {cauHoiId} cho Quiz Ngày.");
            }

            // 2. Tạo QuizTuyChinh mặc định
            var defaultQuiz = new QuizTuyChinh
            {
                UserID = userId,
                TenQuiz = "Quiz Ngày - " + DateTime.Now.ToString("dd/MM/yyyy"),
                MoTa = "Bài Quiz cố định hàng ngày",
                NgayTao = DateTime.Now,
                TrangThai = "Approved" // Đã duyệt vì là Quiz hệ thống
            };

            await _unitOfWork.Quiz.AddQuizTuyChinhAsync(defaultQuiz);
            await _unitOfWork.CompleteAsync();

            // 3. Tạo QuizAttempt
            var quizAttempt = new QuizAttempt
            {
                UserID = userId,
                QuizTuyChinhID = defaultQuiz.QuizTuyChinhID,
                NgayBatDau = DateTime.Now,
                TrangThai = "Đang làm",
                SoCauHoiLam = 0,
                SoCauDung = 0,
                Diem = 0
            };

            await _unitOfWork.Quiz.AddQuizAttemptAsync(quizAttempt);
            await _unitOfWork.CompleteAsync();

            // 4. Lưu session vào memory (chỉ chứa 1 câu hỏi duy nhất)
            _activeSessions[quizAttempt.QuizAttemptID] = new QuizSessionData
            {
                QuizAttempt = quizAttempt,
                Questions = new Queue<CauHoi>(new[] { question }), // Chỉ có 1 câu hỏi
                TotalQuestions = 1,
                CorrectAnswers = 0,
                UserId = userId
            };

            return quizAttempt.QuizAttemptID;
        }

        // 1. Bắt đầu làm bài
        public async Task<int> StartNewQuizAttemptAsync(int userId, GameStartOptions options)
        {
            var questions = (await _unitOfWork.Quiz.GetRandomQuestionsAsync(
                                 options.SoLuongCauHoi, options.ChuDeID, options.DoKhoID))
                                .ToList();

            if (!questions.Any())
                throw new Exception("Không có câu hỏi phù hợp.");

            // ✅ TẠO QuizTuyChinh mặc định trước và LƯU
            var defaultQuiz = new QuizTuyChinh
            {
                UserID = userId,
                TenQuiz = "Quiz Ngẫu Nhiên - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                MoTa = "Quiz tự động tạo",
                NgayTao = DateTime.Now
            };

            await _unitOfWork.Quiz.AddQuizTuyChinhAsync(defaultQuiz);
            await _unitOfWork.CompleteAsync(); // Lưu QuizTuyChinh để lấy QuizTuyChinhID

            // ✅ TẠO QuizAttempt (ĐỂ ID = 0 để DB tự gán)
            var quizAttempt = new QuizAttempt
            {
                // BỎ QuizAttemptID = _attemptIdCounter++,
                UserID = userId,
                QuizTuyChinhID = defaultQuiz.QuizTuyChinhID, // Sử dụng ID vừa lưu
                NgayBatDau = DateTime.Now,
                NgayKetThuc = null,
                TrangThai = "Đang làm",
                SoCauHoiLam = 0,
                SoCauDung = 0,
                Diem = 0
            };

            // ✅ LƯU VÀO DB LẦN ĐẦU TIÊN ĐỂ LẤY ID THẬT
            // YÊU CẦU: IQuizRepository phải có hàm AddQuizAttemptAsync
            await _unitOfWork.Quiz.AddQuizAttemptAsync(quizAttempt);
            await _unitOfWork.CompleteAsync();

            // SAU DÒNG NÀY: quizAttempt.QuizAttemptID đã là ID thật từ DB

            // ✅ Lưu session vào memory bằng ID THẬT
            _activeSessions[quizAttempt.QuizAttemptID] = new QuizSessionData
            {
                QuizAttempt = quizAttempt,
                Questions = new Queue<CauHoi>(questions),
                TotalQuestions = questions.Count,
                CorrectAnswers = 0,
                UserId = userId
            };

            return quizAttempt.QuizAttemptID;
        }

        // ... (Các hàm GetNextQuestionAsync và SubmitAnswerAsync giữ nguyên)

        // 3. Nộp đáp án (Không đổi, vì nó chỉ cập nhật session trong memory và thêm CauSai vào DB)
        public async Task<bool> SubmitAnswerAsync(AnswerSubmitModel answer)
        {
            if (!_activeSessions.ContainsKey(answer.QuizAttemptID))
                throw new Exception("Phiên làm bài không tồn tại.");

            var session = _activeSessions[answer.QuizAttemptID];

            var correct = await _unitOfWork.Quiz.GetCorrectAnswerAsync(answer.CauHoiID);
            bool isCorrect = correct?.Equals(answer.DapAnDaChon, StringComparison.OrdinalIgnoreCase) ?? false;

            if (isCorrect)
            {
                session.CorrectAnswers++;
            }
            else
            {
                var cauSai = new CauSai
                {
                    UserID = answer.UserID,
                    CauHoiID = answer.CauHoiID,
                    QuizAttemptID = answer.QuizAttemptID,
                    NgaySai = DateTime.Now
                };

                await _unitOfWork.Results.AddCauSaiAsync(cauSai);
                await _unitOfWork.CompleteAsync();
            }

            session.QuizAttempt.SoCauHoiLam++;
            session.QuizAttempt.SoCauDung = session.CorrectAnswers;

            return isCorrect;
        }

        // 4. Kết thúc bài
        public async Task<KetQua> EndAttemptAndCalculateResultAsync(int attemptId, int userId)
        {
            if (!_activeSessions.ContainsKey(attemptId))
                throw new Exception("Phiên làm bài không tồn tại.");

            var session = _activeSessions[attemptId];
            int total = session.TotalQuestions;
            int correctCount = session.CorrectAnswers;

            int diem = correctCount * 10;

            // ✅ CẬP NHẬT QuizAttempt
            session.QuizAttempt.NgayKetThuc = DateTime.Now;
            session.QuizAttempt.TrangThai = "Hoàn thành";
            session.QuizAttempt.SoCauHoiLam = total;
            session.QuizAttempt.SoCauDung = correctCount;
            session.QuizAttempt.Diem = diem;

            // ✅ LƯU QuizAttempt vào database (Lúc này là Update bản ghi đã tồn tại)
            await _unitOfWork.Quiz.SaveQuizAttemptAsync(session.QuizAttempt); // Sẽ là UPDATE

            // ✅ TẠO KetQua
            var ketQua = new KetQua
            {
                UserID = userId,
                QuizAttemptID = attemptId,
                Diem = diem,
                SoCauDung = correctCount,
                TongCauHoi = total,
                TrangThaiKetQua = "Hoàn thành",
                ThoiGian = DateTime.Now
            };

            _unitOfWork.Results.AddKetQua(ketQua);
            await _unitOfWork.CompleteAsync();

            // ✅ XÓA session
            _activeSessions.Remove(attemptId);

            return ketQua;
        }

        // ... (QuizSessionData giữ nguyên)
        internal class QuizSessionData
        {
            public QuizAttempt QuizAttempt { get; set; } = null!;
            public Queue<CauHoi> Questions { get; set; } = new Queue<CauHoi>();
            public int TotalQuestions { get; set; }
            public int CorrectAnswers { get; set; }
            public int UserId { get; set; }
        }

        // 2. Lấy câu hỏi tiếp theo (Giữ nguyên)
        public Task<CauHoiPlayDto?> GetNextQuestionAsync(int attemptId)
        {
            if (!_activeSessions.ContainsKey(attemptId))
                return Task.FromResult<CauHoiPlayDto?>(null);

            var session = _activeSessions[attemptId];

            if (session.Questions.Count == 0)
                return Task.FromResult<CauHoiPlayDto?>(null);

            var next = session.Questions.Dequeue();

            return Task.FromResult<CauHoiPlayDto?>(new CauHoiPlayDto
            {
                CauHoiID = next.CauHoiID,
                NoiDung = next.NoiDung,
                DapAnA = next.DapAnA,
                DapAnB = next.DapAnB,
                DapAnC = next.DapAnC,
                DapAnD = next.DapAnD,
                HinhAnh = next.HinhAnh
            });
        }
    }
}