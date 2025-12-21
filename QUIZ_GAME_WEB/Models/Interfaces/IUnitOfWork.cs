using System;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // CORE REPOSITORIES
        IUserRepository Users { get; }
        ITranDauRepository TranDau { get; }
        IQuizRepository Quiz { get; }
        IResultRepository Results { get; }
        ISystemRepository Systems { get; }
        IClientKeyRepository ClientKeys { get; }

        // SPECIALIZED & SOCIAL
        ICommentRepository Comments { get; }
        ILoginSessionRepository LoginSessions { get; }
        IAchievementsRepository Achievements { get; }
        ISocialRepository Social { get; }
        // ✅ THÊM DEFINITION CHO REPOSITORIES QUẢN TRỊ
        IRoleRepository Roles { get; } // Cho VaiTro (Role)
        IPermissionRepository Permissions { get; } // Cho Quyen (Permission)

        // ... (Các Repositories khác) ...

        Task<int> CompleteAsync();

        // SAVE CHANGES
    }
}
