// Models/Implementations/TranDauRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class TranDauRepository : GenericRepository<TranDauTrucTiep>, ITranDauRepository
    {
        // Constructor gọi base(context) để GenericRepository nhận _context
        public TranDauRepository(QuizGameContext context) : base(context) { }

        // ===============================================
        // 1. THAO TÁC LOG TRẢ LỜI
        // ===============================================

        public async Task AddPlayerAnswerAsync(TraLoiTrucTiep answerLog)
        {
            // Thêm log trả lời vào DbSet TraLoiTrucTiep
            await _context.TraLoiTrucTieps.AddAsync(answerLog);
        }

        public async Task<IEnumerable<TraLoiTrucTiep>> GetMatchAnswersAsync(int tranDauId)
        {
            // Lấy tất cả log trả lời của một trận đấu
            return await _context.TraLoiTrucTieps
                                 .Where(t => t.TranDauID == tranDauId)
                                 .ToListAsync();
        }

        // ===============================================
        // 2. QUẢN LÝ CÂU HỎI TRONG TRẬN
        // ===============================================

        public void AddMatchQuestions(IEnumerable<TranDauCauHoi> matchQuestions)
        {
            // Thêm nhiều câu hỏi vào bảng nối TranDau_CauHoi
            _context.TranDauCauHois.AddRange(matchQuestions);
        }
        // Trong Models/Implementations/TranDauRepository.cs
        // (Bổ sung vào class)

        
        public async Task<IEnumerable<CauHoi>> GetMatchQuestionsWithDetailsAsync(int tranDauId)
        {
            // Sử dụng LINQ để JOIN bảng TranDauCauHoi và CauHoi
            var questions = await (from tdc in _context.TranDauCauHois // Tên bảng nối
                                   join ch in _context.CauHois on tdc.CauHoiID equals ch.CauHoiID // Tên bảng Câu Hỏi
                                   where tdc.TranDauID == tranDauId
                                   orderby tdc.ThuTu // Sắp xếp theo thứ tự đã gán
                                   select new CauHoi
                                   {
                                       CauHoiID = ch.CauHoiID,
                                       NoiDung = ch.NoiDung,
                                       CacLuaChon = ch.CacLuaChon,
                                       // === BỔ SUNG CÁC DÒNG DƯỚI ===
                                       DapAnA = ch.DapAnA,
                                       DapAnB = ch.DapAnB,
                                       DapAnC = ch.DapAnC,
                                       DapAnD = ch.DapAnD,
                                       DoKho = ch.DoKho // (Tùy chọn) Để lấy điểm thưởng chính xác
                                                        // ============================
                                   })
                                   .ToListAsync();

            // Lưu ý: Nếu Entity CauHoi có các Navigation Property, bạn cần Include chúng nếu cần thiết.

            // Tuy nhiên, vì bạn chỉ cần dữ liệu cơ bản, select new CauHoi là đủ.
            return questions;
        }
        public async Task<IEnumerable<CauHoi>> GetQuestionsByMatchIdAsync(int tranDauId)
        {
            // Logic: JOIN bảng nối TranDauCauHoi và CauHoi
            return await (from tq in _context.TranDauCauHois
                          join q in _context.CauHois on tq.CauHoiID equals q.CauHoiID
                          where tq.TranDauID == tranDauId
                          orderby tq.ThuTu
                          select q)
                          .ToListAsync();
        }
        // Trong Models/Implementations/TranDauRepository.cs
        // (Sử dụng DbContext _context)

        public async Task<IEnumerable<TraLoiTrucTiep>> GetAllAnswersForMatchAsync(int tranDauId)
        {
            return await _context.TraLoiTrucTieps
                                 .Where(a => a.TranDauID == tranDauId)
                                 .ToListAsync();
        }

        // ===============================================
        // 3. TRUY VẤN VÀ CẬP NHẬT TRẠNG THÁI
        // ===============================================

        public async Task<TranDauTrucTiep?> GetMatchStatusAsync(int tranDauId)
        {
            // Lấy trạng thái trận đấu (cần Include Player 1 & 2 nếu muốn hiển thị tên)
            return await _context.TranDauTrucTieps
                                 .Include(t => t.Player1)
                                 .Include(t => t.Player2)
                                 .FirstOrDefaultAsync(t => t.TranDauID == tranDauId);
        }

        public async Task UpdateMatchResultAsync(int tranDauId, int? winnerId, int diemPlayer1, int diemPlayer2)
        {
            var match = await _context.TranDauTrucTieps.FindAsync(tranDauId);
            if (match == null) return;

            // Cập nhật kết quả cuối cùng
            match.DiemPlayer1 = diemPlayer1;
            match.DiemPlayer2 = diemPlayer2;
            match.WinnerUserID = winnerId;
            match.TrangThai = "HoanThanh";
            match.ThoiGianKetThuc = DateTime.Now;

            _context.TranDauTrucTieps.Update(match);
            // Lưu ý: Việc SaveChangesAsync được gọi từ UnitOfWork
        }
    }
}