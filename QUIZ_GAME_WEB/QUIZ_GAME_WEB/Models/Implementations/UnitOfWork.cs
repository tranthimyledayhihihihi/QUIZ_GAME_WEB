using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.Implementations;
using System;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuizGameContext _context;

        // --- Repository properties (Đã bổ sung Roles và Permissions) ---
        public ITranDauRepository TranDau { get; private set; }
        public IUserRepository Users { get; private set; }
        public IQuizRepository Quiz { get; private set; }
        public IResultRepository Results { get; private set; }
        public ISocialRepository Social { get; private set; }
        public ISystemRepository Systems { get; private set; }
        public IClientKeyRepository ClientKeys { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public ILoginSessionRepository LoginSessions { get; private set; }
        public IAchievementsRepository Achievements { get; private set; }

        // ✅ BỔ SUNG KHAI BÁO THUỘC TÍNH
        public IRoleRepository Roles { get; private set; }
        public IPermissionRepository Permissions { get; private set; }

        public UnitOfWork(QuizGameContext context)
        {
            _context = context;

            // --- Khởi tạo repository (Đã bổ sung Roles và Permissions) ---
            TranDau = new TranDauRepository(_context);
            Users = new UserRepository(_context);
            Quiz = new QuizRepository(_context);
            Results = new ResultRepository(_context);
            Social = new SocialRepository(_context);
            Systems = new SystemRepository(_context);
            ClientKeys = new ClientKeyRepository(_context);
            Comments = new CommentRepository(_context);
            LoginSessions = new LoginSessionRepository(_context);
            Achievements = new AchievementsRepository(_context);

            // ✅ KHỞI TẠO IMPLEMENTATION CỦA REPOSITORIES MỚI
            Roles = new RoleRepository(_context);
            Permissions = new PermissionRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}