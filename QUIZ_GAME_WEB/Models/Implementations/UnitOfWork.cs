// Models/Implementations/UnitOfWork.cs
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuizGameContext _context;

        // --- Khai báo các Repository properties ---

        public IUserRepository Users { get; private set; }
        public IQuizRepository Quiz { get; private set; }
        public IResultRepository Results { get; private set; }
        public ISocialRepository Social { get; private set; }
        public ISystemRepository Systems { get; private set; }
        public IClientKeyRepository ClientKeys { get; private set; }

        // Khai báo các Repository mới (Đã được định nghĩa là get; private set;)
        public ICommentRepository Comments { get; private set; }
        public ILoginSessionRepository LoginSessions { get; private set; }
        public IAchievementsRepository Achievements { get; private set; }

        public UnitOfWork(QuizGameContext context)
        {
            _context = context;

            // --- Khởi tạo các Repository ---
            // Lưu ý: Các Repository (ví dụ: UserRepository, QuizRepository) phải tồn tại 
            // và nhận QuizGameContext trong constructor của chúng.

            Users = new UserRepository(_context);
            Quiz = new QuizRepository(_context);
            Results = new ResultRepository(_context);
            Social = new SocialRepository(_context);
            Systems = new SystemRepository(_context);
            ClientKeys = new ClientKeyRepository(_context);

            // Khởi tạo các Repository mới
            Comments = new CommentRepository(_context);
            LoginSessions = new LoginSessionRepository(_context);
            Achievements = new AchievementsRepository(_context);
        }

        /// <summary>
        /// Thực hiện lưu tất cả các thay đổi (INSERT/UPDATE/DELETE) đang chờ xử lý trong DbContext.
        /// </summary>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Giải phóng tài nguyên DbContext.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}