using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels; // Cần thiết cho GenericRepository<ThanhTuuDefinition> và <ThanhTuu>
using System;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuizGameContext _context;

        // ===============================================
        // KHAI BÁO CÁC BACKING FIELDS CHO REPOSITORIES MỚI/CÓ VẤN ĐỀ
        // ===============================================
        // Khai báo kiểu chung (GenericRepository) để triển khai các thuộc tính mới
        private IGenericRepository<ThanhTuuDefinition>? _achievementDefinitions;
        private IGenericRepository<ThanhTuu>? _userAchievements;

        // --- Repository properties (Đã giữ nguyên kiểu cũ nếu không cần thay đổi) ---
        public ITranDauRepository TranDau { get; private set; }
        public IUserRepository Users { get; private set; }
        public IQuizRepository Quiz { get; private set; }
        public IResultRepository Results { get; private set; }
        public ISocialRepository Social { get; private set; }
        public ISystemRepository Systems { get; private set; }
        public IClientKeyRepository ClientKeys { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public ILoginSessionRepository LoginSessions { get; private set; }

        // ❌ IAchievementsRepository Achievements đã bị loại bỏ/thay thế. 
        // Nếu giữ lại, bạn cần triển khai nó ở đây. Tạm thời tôi loại bỏ nó để tránh xung đột với IUnitOfWork mới.

        public IRoleRepository Roles { get; private set; }
        public IPermissionRepository Permissions { get; private set; }

        // ===============================================
        // ✅ TRIỂN KHAI CÁC PROPERTY MỚI/ĐÃ SỬA ĐỔI
        // ===============================================

        public IGenericRepository<ThanhTuuDefinition> AchievementDefinitions =>
            _achievementDefinitions ??= new GenericRepository<ThanhTuuDefinition>(_context);

        public IGenericRepository<ThanhTuu> UserAchievements =>
            _userAchievements ??= new GenericRepository<ThanhTuu>(_context);

        // ===============================================
        // CONSTRUCTOR
        // ===============================================
        public UnitOfWork(QuizGameContext context)
        {
            _context = context;

            // --- Khởi tạo repository (Giữ nguyên các khởi tạo cũ) ---
            TranDau = new TranDauRepository(_context);
            Users = new UserRepository(_context);
            Quiz = new QuizRepository(_context);
            Results = new ResultRepository(_context);
            Social = new SocialRepository(_context);
            Systems = new SystemRepository(_context);
            ClientKeys = new ClientKeyRepository(_context);
            Comments = new CommentRepository(_context);
            LoginSessions = new LoginSessionRepository(_context);

            // ❌ Loại bỏ khởi tạo IAchievementsRepository cũ nếu nó không còn tồn tại trong IUnitOfWork
            // Achievements = new AchievementsRepository(_context); 

            Roles = new RoleRepository(_context);
            Permissions = new PermissionRepository(_context);

            // GHI CHÚ: Các Repository Generic mới (AchievementDefinitions, UserAchievements) 
            // được khởi tạo lazily trong getter của chúng.
        }

        // ===============================================
        // ACTIONS
        // ===============================================
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