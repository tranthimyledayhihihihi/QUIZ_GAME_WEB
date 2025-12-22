using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class QuizRepository : GenericRepository<CauHoi>, IQuizRepository
    {
        // Khắc phục lỗi "hiding inherited member"
        private new readonly QuizGameContext _context;

        // Khởi tạo các Generic Repository để trả về từ IQuizRepository
        private readonly IGenericRepository<ChuDe> _topicRepository;
        private readonly IGenericRepository<DoKho> _difficultyRepository;

        public QuizRepository(QuizGameContext context) : base(context)
        {
            _context = context;
            // Khởi tạo các Generic Repository cho các Entity liên quan
            _topicRepository = new GenericRepository<ChuDe>(context);
            _difficultyRepository = new GenericRepository<DoKho>(context);
        }

        // ===============================================
        // I. CÁC HÀM TRUY VẤN CƠ BẢN & CHUYÊN BIỆT
        // ===============================================

        public async Task<IEnumerable<CauHoi>> GetRandomQuestionsAsync(int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHois.AsQueryable();
            if (chuDeId.HasValue) query = query.Where(q => q.ChuDeID == chuDeId.Value);
            if (doKhoId.HasValue) query = query.Where(q => q.DoKhoID == doKhoId.Value);
            query = query.Where(q => q.TrangThaiDuyet == "Approved"); // Chỉ lấy câu hỏi đã duyệt

            query = query.Include(q => q.ChuDe).Include(q => q.DoKho);
            return await query.OrderBy(r => Guid.NewGuid()).Take(count).ToListAsync();
        }

        public async Task<string?> GetCorrectAnswerAsync(int cauHoiId)
        {
            return await _context.CauHois.Where(q => q.CauHoiID == cauHoiId).Select(q => q.DapAnDung).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ChuDe>> GetAllTopicsAsync() => await _context.ChuDes.ToListAsync();
        public async Task<IEnumerable<DoKho>> GetAllDifficultiesAsync() => await _context.DoKhos.ToListAsync();

        public async Task<CauHoiInfoDto?> GetQuestionDetailByIdAsync(int cauHoiId)
        {
            return await _context.CauHois
                .Where(q => q.CauHoiID == cauHoiId)
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
                    DiemThuong = q.DoKho.DiemThuong,
                    TrangThaiDuyet = q.TrangThaiDuyet
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        // ===============================================
        // II. CÁC HÀM THAO TÁC (CRUD & Transaction Support)
        // ===============================================

        public void AddTopic(ChuDe topic) => _context.ChuDes.Add(topic);
        public async Task AddQuizTuyChinhAsync(QuizTuyChinh quiz) => await _context.QuizTuyChinhs.AddAsync(quiz);
        public async Task AddQuizAttemptAsync(QuizAttempt attempt) => await _context.QuizAttempts.AddAsync(attempt);

        // Sửa: Hàm SaveQuizAttemptAsync trả về Task (async void không được dùng)
        public Task SaveQuizAttemptAsync(QuizAttempt attempt)
        {
            _context.QuizAttempts.Update(attempt);
            return Task.CompletedTask;
        }

        public async Task AddQuizChiaSeAsync(QuizChiaSe share) => await _context.QuizChiaSes.AddAsync(share);

        // ===============================================
        // III. HÀM MODERATION & REPOSITORY ACCESS (TRIỂN KHAI CÁC MEMBER CÒN THIẾU)
        // ===============================================

        // ✅ FIX LỖI THIẾU: GetPendingQuestionsAsync()
        public async Task<IQueryable<CauHoi>> GetPendingQuestionsAsync()
        {
            return _context.CauHois
                           .Where(q => q.TrangThaiDuyet == "Pending")
                           .AsQueryable();
        }

        // ✅ FIX LỖI THIẾU: ApproveQuestionAsync(int, int)
        public async Task<bool> ApproveQuestionAsync(int cauHoiId, int adminId)
        {
            var question = await _context.CauHois.FindAsync(cauHoiId);

            if (question == null || question.TrangThaiDuyet != "Pending")
            {
                return false;
            }

            question.TrangThaiDuyet = "Approved";
            question.AdminDuyetID = adminId;
            question.NgayTao = DateTime.UtcNow;

            _context.CauHois.Update(question);
            return true;
        }

        // ✅ FIX LỖI THIẾU: GetTopicRepository()
        public IGenericRepository<ChuDe> GetTopicRepository()
        {
            return _topicRepository;
        }

        // ✅ FIX LỖI THIẾU: GetDifficultyRepository()
        public IGenericRepository<DoKho> GetDifficultyRepository()
        {
            return _difficultyRepository;
        }

        public async Task<int> CountAllCauHoisAsync() => await _context.CauHois.CountAsync();
        public async Task<int> CountActiveQuestionsAsync() => await _context.CauHois.Where(q => q.TrangThaiDuyet == "Approved").CountAsync();

        // ===============================================
        // IV. CÁC HÀM TRUY VẤN TỐI ƯU HÓA CHO API (Projection)
        // ===============================================

        public async Task<IEnumerable<CauHoiInfoDto>> GetRandomQuestionsWithDetailsAsync(
            int count, int? chuDeId, int? doKhoId)
        {
            var query = _context.CauHois.Include(q => q.ChuDe).Include(q => q.DoKho).AsQueryable();

            if (chuDeId.HasValue) query = query.Where(q => q.ChuDeID == chuDeId.Value);
            if (doKhoId.HasValue) query = query.Where(q => q.DoKhoID == doKhoId.Value);
            query = query.Where(q => q.TrangThaiDuyet == "Approved");

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

        public async Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetQuestionsFilteredAsync(
            int pageNumber, int pageSize, string? keyword = null, int? chuDeId = null, int? doKhoId = null)
        {
            var query = _context.CauHois.Include(q => q.ChuDe).Include(q => q.DoKho).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(q => q.NoiDung.Contains(keyword) || q.DapAnA.Contains(keyword) || q.DapAnB.Contains(keyword) ||
                                         q.DapAnC.Contains(keyword) || q.DapAnD.Contains(keyword));
            }
            if (chuDeId.HasValue) query = query.Where(q => q.ChuDeID == chuDeId.Value);
            if (doKhoId.HasValue) query = query.Where(q => q.DoKhoID == doKhoId.Value);

            var totalCount = await query.CountAsync();

            var questions = await query
                .OrderBy(q => q.CauHoiID)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
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
                    DiemThuong = q.DoKho.DiemThuong,
                    TrangThaiDuyet = q.TrangThaiDuyet
                })
                .ToListAsync();

            return (questions, totalCount);
        }

        public async Task<(IEnumerable<CauHoiInfoDto> Questions, int TotalCount)> GetIncorrectQuestionsByUserIdAsync(
            int userId, int pageNumber, int pageSize)
        {
            var incorrectQuestionIds = _context.CauSais.Where(cs => cs.UserID == userId).Select(cs => cs.CauHoiID).Distinct().AsQueryable();
            var totalCount = await incorrectQuestionIds.CountAsync();
            var pagedQuestionIds = await incorrectQuestionIds.OrderBy(id => id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var questions = await _context.CauHois
                .Where(q => pagedQuestionIds.Contains(q.CauHoiID))
                .Include(q => q.ChuDe).Include(q => q.DoKho)
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

        public async Task<IEnumerable<CauHoi>> GetAllCauHoisWithDetailsAsync()
        {
            return await _context.CauHois.Include(q => q.ChuDe).Include(q => q.DoKho).AsNoTracking().ToListAsync();
        }

        // ===============================================
        // V. HÀM UGC (User-Generated Content) & SHARE
        // ===============================================

        public async Task<(IEnumerable<QuizTuyChinhDto> Quizzes, int TotalCount)> GetQuizSubmissionsByUserIdAsync(
            int userId, int pageNumber, int pageSize)
        {
            var query = _context.QuizTuyChinhs
                .Where(q => q.UserID == userId)
                .OrderByDescending(q => q.NgayTao)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var quizzes = await query
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(q => new QuizTuyChinhDto
                {
                    QuizTuyChinhID = q.QuizTuyChinhID,
                    TenQuiz = q.TenQuiz,
                    MoTa = q.MoTa,
                    NgayTao = q.NgayTao,
                    TrangThai = q.TrangThai,
                    SoCauHoi = q.CauHois.Count()
                })
                .ToListAsync();

            return (quizzes, totalCount);
        }

        public async Task<QuizTuyChinh?> GetQuizSubmissionByIdAsync(int quizId)
        {
            return await _context.QuizTuyChinhs
                .Include(q => q.CauHois).ThenInclude(c => c.ChuDe)
                .FirstOrDefaultAsync(q => q.QuizTuyChinhID == quizId);
        }

        public async Task<QuizTuyChinh> SubmitNewQuizAsync(int userId, QuizSubmissionModel submission)
        {
            var newQuiz = new QuizTuyChinh
            {
                UserID = userId,
                TenQuiz = submission.TenQuiz,
                MoTa = submission.MoTa,
                NgayTao = DateTime.Now,
                TrangThai = "Pending"
            };

            await _context.QuizTuyChinhs.AddAsync(newQuiz);
            await _context.SaveChangesAsync(); // Lưu QuizTuyChinh để lấy ID

            var questionsToSubmit = submission.Questions.Select(qModel => new CauHoi
            {
                ChuDeID = qModel.ChuDeID,
                DoKhoID = qModel.DoKhoID,
                NoiDung = qModel.NoiDung,
                DapAnA = qModel.DapAnA,
                DapAnB = qModel.DapAnB,
                DapAnC = qModel.DapAnC,
                DapAnD = qModel.DapAnD,
                DapAnDung = qModel.DapAnDung,
                HinhAnh = qModel.HinhAnh,
                NgayTao = DateTime.Now,
                TrangThaiDuyet = "Pending",
                QuizTuyChinhID = newQuiz.QuizTuyChinhID // Gán FK
            }).ToList();

            await _context.CauHois.AddRangeAsync(questionsToSubmit);
            return newQuiz;
        }

        public async Task<bool> DeleteQuizSubmissionAsync(int quizId, int userId)
        {
            var quizToDelete = await _context.QuizTuyChinhs
                .Include(q => q.CauHois)
                .FirstOrDefaultAsync(q => q.QuizTuyChinhID == quizId && q.UserID == userId);

            if (quizToDelete == null || quizToDelete.TrangThai != "Pending")
            {
                return false;
            }

            _context.CauHois.RemoveRange(quizToDelete.CauHois);
            _context.QuizTuyChinhs.Remove(quizToDelete);
            return true;
        }

        public async Task<bool> CheckQuizOwnershipAndExistenceAsync(int quizId, int userId)
        {
            return await _context.QuizTuyChinhs
                .AnyAsync(q => q.QuizTuyChinhID == quizId && q.UserID == userId);
        }

        // Hàm chia sẻ: Đã sửa lỗi LINQ .First()
        public async Task<(IEnumerable<QuizShareDto> Shares, int TotalCount)> GetSharedQuizzesBySenderAsync(int userId)
        {
            var query = _context.QuizChiaSes
                .Where(share => share.UserGuiID == userId)
                .OrderByDescending(share => share.NgayChiaSe)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var shares = await query
                .Join(_context.QuizTuyChinhs, s => s.QuizTuyChinhID, q => q.QuizTuyChinhID, (s, q) => new { s, q })
                .Join(_context.NguoiDungs, joined => joined.s.UserNhanID, receiver => receiver.UserID, (joined, receiver) => new QuizShareDto
                {
                    QuizChiaSeID = joined.s.QuizChiaSeID,
                    QuizTuyChinhID = joined.s.QuizTuyChinhID,
                    TenQuiz = joined.q.TenQuiz,
                    NgayChiaSe = joined.s.NgayChiaSe,
                    UserGuiID = joined.s.UserGuiID,
                    UserNhanID = joined.s.UserNhanID ?? 0,
                    TenNguoiNhan = receiver.HoTen,
                    TenNguoiGui = _context.NguoiDungs.Where(u => u.UserID == joined.s.UserGuiID).Select(u => u.HoTen).FirstOrDefault()
                })
                .ToListAsync();

            return (shares, totalCount);
        }

        public async Task<(IEnumerable<QuizShareDto> Shares, int TotalCount)> GetSharedQuizzesByReceiverAsync(int userId)
        {
            var query = _context.QuizChiaSes
                .Where(share => share.UserNhanID == userId)
                .OrderByDescending(share => share.NgayChiaSe)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var shares = await query
                .Join(_context.QuizTuyChinhs, s => s.QuizTuyChinhID, q => q.QuizTuyChinhID, (s, q) => new { s, q })
                .Join(_context.NguoiDungs, joined => joined.s.UserGuiID, sender => sender.UserID, (joined, sender) => new QuizShareDto
                {
                    QuizChiaSeID = joined.s.QuizChiaSeID,
                    QuizTuyChinhID = joined.s.QuizTuyChinhID,
                    TenQuiz = joined.q.TenQuiz,
                    NgayChiaSe = joined.s.NgayChiaSe,
                    UserGuiID = joined.s.UserGuiID,
                    UserNhanID = joined.s.UserNhanID ?? 0,
                    TenNguoiGui = sender.HoTen,
                    TenNguoiNhan = _context.NguoiDungs.Where(u => u.UserID == joined.s.UserNhanID).Select(u => u.HoTen).FirstOrDefault()
                })
                .ToListAsync();

            return (shares, totalCount);
        }

        public async Task<QuizShareDetailDto?> GetShareDetailByIdAsync(int shareId)
        {
            var shareDetail = await _context.QuizChiaSes
                .Where(s => s.QuizChiaSeID == shareId)
                .Join(_context.QuizTuyChinhs, s => s.QuizTuyChinhID, q => q.QuizTuyChinhID, (s, q) => new { s, q })
                .Join(_context.NguoiDungs, joined => joined.s.UserGuiID, sender => sender.UserID, (joined, sender) => new { joined.s, joined.q, sender })
                .Join(_context.NguoiDungs, joined => joined.s.UserNhanID, receiver => receiver.UserID, (joined, receiver) => new QuizShareDetailDto
                {
                    QuizChiaSeID = joined.s.QuizChiaSeID,
                    NgayChiaSe = joined.s.NgayChiaSe,
                    UserGuiID = joined.s.UserGuiID,
                    TenNguoiGui = joined.sender.HoTen,
                    UserNhanID = joined.s.UserNhanID ?? 0,
                    TenNguoiNhan = receiver.HoTen,
                    QuizMetadata = new QuizTuyChinhMetadataDto
                    {
                        QuizTuyChinhID = joined.q.QuizTuyChinhID,
                        TenQuiz = joined.q.TenQuiz,
                        MoTa = joined.q.MoTa,
                        TrangThai = joined.q.TrangThai,
                        TongSoCauHoi = _context.CauHois.Count(c => c.QuizTuyChinhID == joined.q.QuizTuyChinhID)
                    }
                })
                .FirstOrDefaultAsync();

            return shareDetail;
        }

        public async Task<QuizNgayDetailsDto?> GetTodayQuizDetailsAsync()
        {
            var todayQuiz = await _context.QuizNgays
                .Where(qn => qn.Ngay.Date == DateTime.Today.Date)
                .Include(qn => qn.CauHoi)
                    .ThenInclude(ch => ch!.DoKho)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (todayQuiz == null || todayQuiz.CauHoi == null || todayQuiz.CauHoi.DoKho == null)
            {
                return null;
            }

            return new QuizNgayDetailsDto
            {
                QuizNgayID = todayQuiz.QuizNgayID,
                Ngay = todayQuiz.Ngay,
                CauHoiID = (int)todayQuiz.CauHoiID, // Assuming CauHoiID is int? in QuizNgay
                NoiDungCauHoi = todayQuiz.CauHoi.NoiDung,
                TenDoKho = todayQuiz.CauHoi.DoKho.TenDoKho,
                DiemThuong = todayQuiz.CauHoi.DoKho.DiemThuong
                // Bổ sung các trường DapAnA, B, C, D nếu DTO QuizNgayDetailsDto yêu cầu
            };
        }
        // ================================
        public IGenericRepository<TroGiup> GetHelperRepository()
        {
            return new GenericRepository<TroGiup>(_context);
        }

        /// <summary>
        /// Trả về Generic Repository cho thực thể SystemSettings (Cấu hình hệ thống).
        /// </summary>
        public IGenericRepository<SystemSetting> GetSettingsRepository()
        {
            return new GenericRepository<SystemSetting>(_context);
        }
        // QUIZ TÙY CHỈNH – ADMIN
        // ================================

        public IQueryable<QuizTuyChinh> GetQuizTuyChinhQueryable()
        {
            return _context.QuizTuyChinhs
                .Include(q => q.NguoiDung)
                .Include(q => q.CauHois);
        }

        public async Task<QuizTuyChinh?> GetQuizTuyChinhByIdAsync(int id)
        {
            return await _context.QuizTuyChinhs
                .Include(q => q.NguoiDung)
                .Include(q => q.CauHois)
                .FirstOrDefaultAsync(q => q.QuizTuyChinhID == id);
        }

        public void UpdateQuizTuyChinh(QuizTuyChinh quiz)
        {
            _context.QuizTuyChinhs.Update(quiz);
        }
        public IGenericRepository<ThuongNgay> GetRewardRepository()
        {
            return new GenericRepository<ThuongNgay>(_context);
        }
        public IGenericRepository<Comment> GetCommentRepository()
        {
            return new GenericRepository<Comment>(_context);
        }
        public void DeleteQuizTuyChinh(QuizTuyChinh quiz)
        {
            _context.QuizTuyChinhs.Remove(quiz);
        }

    }
}