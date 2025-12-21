using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels; // Cần cho KetQuaDto và KetQuaDetailDto
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ResultRepository : GenericRepository<KetQua>, IResultRepository
    {
        // Sử dụng _context từ GenericRepository hoặc khai báo lại tùy thuộc vào cấu trúc base
        private readonly QuizGameContext _context;

        public ResultRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // ===============================================
        // I. CÁC HÀM TRUY VẤN CƠ BẢN & THAO TÁC
        // ===============================================

        public async Task<ChuoiNgay?> GetUserStreakAsync(int userId)
        {
            return await _context.ChuoiNgays.FirstOrDefaultAsync(c => c.UserID == userId);
        }
        public void AddStreak(ChuoiNgay streak) => _context.ChuoiNgays.Add(streak);
        public void Update(ChuoiNgay streak) => _context.ChuoiNgays.Update(streak);
        public void AddKetQua(KetQua ketQua) => _context.KetQuas.Add(ketQua);
        public async Task AddCauSaiAsync(CauSai cauSai) => await _context.CauSais.AddAsync(cauSai);
        public async Task AddWrongAnswerAsync(CauSai cauSai) => await AddCauSaiAsync(cauSai);

        // ===============================================
        // II. CÁC HÀM LỊCH SỬ CHƠI (Hỗ trợ Controller)
        // ===============================================

        /// <summary>
        /// Lấy danh sách kết quả bài làm của người dùng (có phân trang).
        /// </summary>
        public async Task<(IEnumerable<KetQuaDto> Results, int TotalCount)> GetPaginatedResultsAsync(
            int userId, int pageNumber, int pageSize)
        {
            var query = _context.KetQuas
                                .Where(kq => kq.UserID == userId)
                                .OrderByDescending(kq => kq.ThoiGian) // Sắp xếp theo thời gian mới nhất
                                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(kq => new KetQuaDto
                {
                    QuizAttemptID = kq.QuizAttemptID,
                    Diem = kq.Diem,
                    SoCauDung = kq.SoCauDung,
                    TongCauHoi = kq.TongCauHoi,
                    TrangThaiKetQua = kq.TrangThaiKetQua
                    // Giả định KetQuaDto không chứa thời gian
                })
                .ToListAsync();

            return (results, totalCount);
        }

        /// <summary>
        /// Lấy chi tiết kết quả bài làm (KetQuaDetailDto) dựa trên Attempt ID.
        /// </summary>
        public async Task<KetQuaDetailDto?> GetResultDetailByAttemptIdAsync(int attemptId, int userId)
        {
            // 1. Tải KetQua và các CauSai liên quan
            var ketQua = await _context.KetQuas
                .Include(kq => kq.QuizAttempt)
                    .ThenInclude(qa => qa!.CauSais) // Lấy các log câu sai
                .FirstOrDefaultAsync(kq => kq.QuizAttemptID == attemptId && kq.UserID == userId);

            if (ketQua == null) return null;

            // 2. Lấy thông tin chi tiết các Câu Hỏi (Câu đúng và Câu sai)
            // Lấy tất cả CauHoiID trong Attempt này (Giả định QuizAttempt có thể lấy danh sách câu hỏi)
            // Vì không có bảng liên kết trung gian (QuizAttempt_CauHoi), ta cần truy vấn ngược.

            // Lấy ID các câu hỏi đã sai
            var incorrectQuestionIds = ketQua.QuizAttempt!.CauSais
                .Select(cs => cs.CauHoiID)
                .ToList();

            // ⚠️ ĐÂY LÀ PHẦN PHỨC TẠP: Cần tái tạo lại toàn bộ QuizAttempt từ đầu để biết câu trả lời đúng
            // Tạm thời bỏ qua logic tái tạo toàn bộ (đúng + sai) để tập trung vào logic Repository chính

            // 3. Ánh xạ dữ liệu sang KetQuaDetailDto
            var detailDto = new KetQuaDetailDto
            {
                QuizAttemptID = ketQua.QuizAttemptID,
                Diem = ketQua.Diem,
                SoCauDung = ketQua.SoCauDung,
                TongCauHoi = ketQua.TongCauHoi,
                TrangThaiKetQua = ketQua.TrangThaiKetQua,
                NgayKetThuc = ketQua.ThoiGian,

                // MOCK ChiTietCauHoi: Chỉ trả về thông tin câu sai đơn giản
                ChiTietCauHoi = ketQua.QuizAttempt.CauSais
                    .Select(cs => new CauHoiDaLamDto // Cần truy vấn CauHoi để lấy nội dung chi tiết
                    {
                        CauHoiID = cs.CauHoiID,
                        DapAnDung = "Cần truy vấn từ CauHoi",
                        LaCauTraLoiDung = false,
                        // Các trường khác bị thiếu do không có thông tin chi tiết câu trả lời người dùng
                    })
                    .ToList()
            };

            // LƯU Ý: Hàm này cần phải được tinh chỉnh sau khi bạn thiết lập cách lưu TRẢ LỜI CỦA NGƯỜI CHƠI (PlayerAnswer)
            // Nếu không có bảng PlayerAnswer, bạn không thể tái tạo lại các câu trả lời đúng của người chơi.

            return detailDto;
        }

        // ===============================================
        // III. CÁC HÀM TRUY VẤN KHÁC (Thống kê/Achievement)
        // ===============================================

        public async Task<IEnumerable<CauSai>> GetRecentWrongAnswersAsync(int userId, int limit = 10)
        {
            return await _context.CauSais
                                 .Where(c => c.UserID == userId)
                                 .OrderByDescending(c => c.NgaySai)
                                 .Take(limit)
                                 .ToListAsync();
        }

        public async Task<int> CountWrongAnswersAsync(int userId, int attemptId)
        {
            return await _context.CauSais.CountAsync(c => c.UserID == userId && c.QuizAttemptID == attemptId);
        }

        public async Task<ThuongNgay?> GetDailyRewardByDateAsync(int userId, DateTime today)
        {
            return await _context.ThuongNgays.FirstOrDefaultAsync(t => t.UserID == userId && t.NgayNhan.Date == today.Date);
        }

        public void AddDailyReward(ThuongNgay newReward)
        {
            _context.ThuongNgays.Add(newReward);
        }

        public async Task<IEnumerable<ThongKeNguoiDung>> GetUserDailyStatsAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Today.AddDays(-30);
            DateTime end = endDate ?? DateTime.Today;

            return await _context.ThongKeNguoiDungs
                                 .Where(t => t.UserID == userId && t.Ngay >= start.Date && t.Ngay <= end.Date)
                                 .OrderBy(t => t.Ngay)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId)
        {
            return await _context.ThanhTuus
                                 .Where(t => t.NguoiDungID == userId)
                                 .ToListAsync();
        }
    }
}