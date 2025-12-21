using System;
using System.Threading.Tasks;
using QUIZ_GAME_WEB.Models.ResultsModels; // Cần cho ThanhTuu, ThanhTuuDefinition

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // ===============================================
        // CORE REPOSITORIES
        // ===============================================

        IUserRepository Users { get; }
        ITranDauRepository TranDau { get; }
        IQuizRepository Quiz { get; }
        IResultRepository Results { get; }
        ISystemRepository Systems { get; }
        IClientKeyRepository ClientKeys { get; }

        // ===============================================
        // SPECIALIZED & SOCIAL
        // ===============================================

        ICommentRepository Comments { get; }

        // 💡 Bổ sung setter nếu cần thay đổi phiên đăng nhập
        ILoginSessionRepository LoginSessions { get; }

        // ❌ IAchievementsRepository Achievements { get; } // Bị loại bỏ để thay bằng 2 property dưới

        // ✅ 1. ACHIEVEMENTS DEFINITIONS (Dùng cho Admin CRUD)
        /// <summary>Repository quản lý Định nghĩa Thành tựu Chung.</summary>
        IGenericRepository<ThanhTuuDefinition> AchievementDefinitions { get; }

        // ✅ 2. USER ACHIEVEMENTS (Dùng cho Logic Game/Kiểm tra FK)
        /// <summary>Repository quản lý Bản ghi Người dùng Đạt Thành tựu (ThanhTuu).</summary>
        IGenericRepository<ThanhTuu> UserAchievements { get; }

        ISocialRepository Social { get; }

        // ✅ REPOSITORIES QUẢN TRỊ
        IRoleRepository Roles { get; } // Cho VaiTro (Role)
        IPermissionRepository Permissions { get; } // Cho Quyen (Permission)

        // 💡 CÓ THỂ CẦN: Repository cho Admin (Nếu Admin không nằm trong Users)
        // IAdminRepository Admins { get; } 

        // ===============================================
        // ACTIONS
        // ===============================================

        /// <summary>
        /// Lưu tất cả các thay đổi đang chờ xử lý vào cơ sở dữ liệu.
        /// </summary>
        /// <returns>Số lượng bản ghi bị ảnh hưởng.</returns>
        Task<int> CompleteAsync();
    }
}