using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.ViewModels; // Cần thiết cho CauHoiInfoDto
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository và thực thi IQuizRepository
    public class QuizRepository : GenericRepository<CauHoi>, IQuizRepository
    {
        // Constructor gọi base(context) để nhận DbContext
        public QuizRepository(QuizGameContext context) : base(context) { }

        // ===============================================
        // I. CÁC HÀM TRUY VẤN CƠ BẢN & CHUYÊN BIỆT
        // ===============================================

        /// <summary>
        /// Lấy câu hỏi ngẫu nhiên (sử dụng cho QuizAttemptService)
        /// </summary>
        public async Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHois.AsQueryable();

            if (chuDeId.HasValue)
            {
                query = query.Where(q => q.ChuDeID == chuDeId.Value);
            }
            if (doKhoId.HasValue)
            {
                query = query.Where(q => q.DoKhoID == doKhoId.Value);
            }

            // Thực hiện Include để tránh lỗi N+1 khi sử dụng các Navigation Property
            query = query.Include(q => q.ChuDe).Include(q => q.DoKho);

            return await query
                         .OrderBy(r => Guid.NewGuid())
                         .Take(count)
                         .ToListAsync();
        }

        /// <summary>
        /// Lấy đáp án đúng của một câu hỏi.
        /// </summary>
        public async Task<string?> GetCorrectAnswerAsync(int cauHoiId)
        {
            return await _context.CauHois
                                 .Where(q => q.CauHoiID == cauHoiId)
                                 .Select(q => q.DapAnDung)
                                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy danh sách tất cả Chủ đề.
        /// </summary>
        public async Task<IEnumerable<ChuDe>> GetAllTopicsAsync()
        {
            return await _context.ChuDes.ToListAsync();
        }

        // ===============================================
        // II. CÁC HÀM THAO TÁC (CRUD & Transaction Support)
        // ===============================================

        /// <summary>
        /// Thêm Chủ đề mới.
        /// </summary>
        public void AddTopic(ChuDe topic)
        {
            _context.ChuDes.Add(topic);
        }

        /// <summary>
        /// Thêm Quiz Tùy chỉnh mới.
        /// </summary>
        public async Task AddQuizTuyChinhAsync(QuizTuyChinh quiz)
        {
            await _context.QuizTuyChinhs.AddAsync(quiz);
        }

        /// <summary>
        /// Thêm QuizAttempt mới (dùng khi Start để lấy ID thật).
        /// </summary>
        public async Task AddQuizAttemptAsync(QuizAttempt attempt)
        {
            await _context.QuizAttempts.AddAsync(attempt);
        }

        /// <summary>
        /// Cập nhật QuizAttempt (dùng khi End bài làm).
        /// </summary>
        public async Task SaveQuizAttemptAsync(QuizAttempt attempt)
        {
            // Update Entity đang được Context theo dõi
            _context.QuizAttempts.Update(attempt);
        }

        // ===============================================
        // III. CÁC HÀM TRUY VẤN TỐI ƯU HÓA CHO API (Controller)
        // ===============================================

        /// <summary>
        /// Lấy câu hỏi ngẫu nhiên, đã ánh xạ sang DTO (Hỗ trợ API random).
        /// </summary>
        public async Task<IEnumerable<CauHoiInfoDto>> GetRandomQuestionsWithDetailsAsync(
            int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHois
                                .Include(q => q.ChuDe)
                                .Include(q => q.DoKho)
                                .AsQueryable();

            if (chuDeId.HasValue)
                query = query.Where(q => q.ChuDeID == chuDeId.Value);

            if (doKhoId.HasValue)
                query = query.Where(q => q.DoKhoID == doKhoId.Value);

            // Ánh xạ DTO (Select) được thực hiện trong DB
            return await query
                         .OrderBy(r => Guid.NewGuid())
                         .Take(count)
                         .Select(q => new CauHoiInfoDto
                         {
                             CauHoiID = q.CauHoiID,
                             NoiDung = q.NoiDung,
                             DapAnA = q.DapAnA,
                             DapAnB = q.DapAnB,
                             DapAnC = q.DapAnC,
                             DapAnD = q.DapAnD,
                             HinhAnh = q.HinhAnh,
                             ChuDeID = q.ChuDeID,
                             TenChuDe = q.ChuDe!.TenChuDe,
                             DoKhoID = q.DoKhoID,
                             TenDoKho = q.DoKho!.TenDoKho,
                             DiemThuong = q.DoKho.DiemThuong
                         })
                         .ToListAsync();
        }

        /// <summary>
        /// Lọc, tìm kiếm và phân trang câu hỏi (Hỗ trợ API search/by-chude/by-dokho).
        /// </summary>
        public async Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetQuestionsFilteredAsync(
            int pageNumber, int pageSize, string? keyword = null, int? chuDeId = null, int? doKhoId = null)
        {
            var query = _context.CauHois
                                .Include(q => q.ChuDe)
                                .Include(q => q.DoKho)
                                .AsQueryable();

            // 1. Lọc theo keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(q => q.NoiDung.Contains(keyword) ||
                                         q.DapAnA.Contains(keyword) ||
                                         q.DapAnB.Contains(keyword) ||
                                         q.DapAnC.Contains(keyword) ||
                                         q.DapAnD.Contains(keyword));
            }

            // 2. Lọc theo Chủ đề
            if (chuDeId.HasValue)
            {
                query = query.Where(q => q.ChuDeID == chuDeId.Value);
            }

            // 3. Lọc theo Độ khó
            if (doKhoId.HasValue)
            {
                query = query.Where(q => q.DoKhoID == doKhoId.Value);
            }

            // 4. Đếm tổng số câu hỏi phù hợp (trước khi phân trang)
            var totalCount = await query.CountAsync();

            // 5. Phân trang và ánh xạ sang DTO
            var questions = await query
                .OrderBy(q => q.CauHoiID) // Cần sắp xếp trước khi phân trang
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new CauHoiInfoDto
                {
                    CauHoiID = q.CauHoiID,
                    NoiDung = q.NoiDung,
                    DapAnA = q.DapAnA,
                    DapAnB = q.DapAnB,
                    DapAnC = q.DapAnC,
                    DapAnD = q.DapAnD,
                    HinhAnh = q.HinhAnh,
                    ChuDeID = q.ChuDeID,
                    TenChuDe = q.ChuDe!.TenChuDe,
                    DoKhoID = q.DoKhoID,
                    TenDoKho = q.DoKho!.TenDoKho,
                    DiemThuong = q.DoKho.DiemThuong
                })
                .ToListAsync();

            return (questions, totalCount);
        }

        /// <summary>
        /// Lấy tất cả câu hỏi kèm chi tiết Chủ đề/Độ khó (Hỗ trợ API statistics).
        /// </summary>
        public async Task<IEnumerable<CauHoi>> GetAllCauHoisWithDetailsAsync()
        {
            return await _context.CauHois
                                 .Include(q => q.ChuDe)
                                 .Include(q => q.DoKho)
                                 .AsNoTracking() // Không cần tracking vì chỉ dùng để thống kê
                                 .ToListAsync();
        }

        /// <summary>
        /// Đếm tổng số câu hỏi trong DB (Hỗ trợ API total-count).
        /// </summary>
        public async Task<int> CountAllCauHoisAsync()
        {
            return await _context.CauHois.CountAsync();
        }

        // ===============================================
        // IV. PHƯƠNG THỨC TRUY VẤN KHÁC (Đã giữ lại)
        // ===============================================

        public async Task<IEnumerable<DoKho>> GetAllDifficultiesAsync()
        {
            return await _context.DoKhos.ToListAsync();
        }

        public async Task<int> CountActiveQuestionsAsync()
        {
            return await _context.CauHois.CountAsync();
        }
    }
}