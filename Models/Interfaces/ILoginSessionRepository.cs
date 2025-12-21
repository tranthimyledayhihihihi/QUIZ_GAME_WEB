// =====================================================================
// File: Models/Interfaces/ILoginSessionRepository.cs
// Mục đích: Interface cho Repository quản lý phiên đăng nhập
// Tác giả: QUIZ_GAME_WEB
// Ngày tạo: 2025
// =====================================================================

using QUIZ_GAME_WEB.Models.CoreEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    /// <summary>
    /// Interface cho LoginSessionRepository - Quản lý phiên đăng nhập người dùng.
    /// Kế thừa các hàm CRUD cơ bản từ IGenericRepository<PhienDangNhap>.
    /// </summary>
    public interface ILoginSessionRepository : IGenericRepository<PhienDangNhap>
    {
        // =====================================================================
        // PHẦN 1: QUẢN LÝ LỊCH SỬ ĐĂNG NHẬP
        // =====================================================================

        /// <summary>
        /// Lấy lịch sử đăng nhập gần nhất của một người dùng.
        /// Sắp xếp theo thời gian đăng nhập giảm dần (mới nhất trước).
        /// </summary>
        /// <param name="userId">ID của người dùng cần lấy lịch sử.</param>
        /// <param name="limit">Số lượng bản ghi tối đa muốn lấy (mặc định 20).</param>
        /// <returns>Danh sách các phiên đăng nhập gần nhất.</returns>
        /// <example>
        /// var history = await _repo.GetLoginHistoryAsync(userId: 123, limit: 10);
        /// </example>
        Task<IEnumerable<PhienDangNhap>> GetLoginHistoryAsync(int userId, int limit = 20);

        /// <summary>
        /// Lấy phiên đăng nhập cuối cùng (gần nhất) của một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Phiên đăng nhập cuối cùng hoặc null nếu không tìm thấy.</returns>
        /// <example>
        /// var lastSession = await _repo.GetLastLoginSessionAsync(userId: 123);
        /// if (lastSession != null) { ... }
        /// </example>
        Task<PhienDangNhap?> GetLastLoginSessionAsync(int userId);

        /// <summary>
        /// Đếm tổng số lần đăng nhập của một người dùng (bao gồm cả phiên đã hết hạn).
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Tổng số phiên đăng nhập đã tạo.</returns>
        /// <example>
        /// int totalLogins = await _repo.CountUserLoginAttemptsAsync(userId: 123);
        /// </example>
        Task<int> CountUserLoginAttemptsAsync(int userId);

        // =====================================================================
        // PHẦN 2: QUẢN LÝ PHIÊN ĐĂNG NHẬP HIỆN TẠI
        // =====================================================================

        /// <summary>
        /// Lấy tất cả các phiên đăng nhập còn hiệu lực (chưa hết hạn và TrangThai = true).
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Danh sách các phiên đăng nhập đang active.</returns>
        /// <example>
        /// var activeSessions = await _repo.GetActiveSessionsAsync(userId: 123);
        /// Console.WriteLine($"User có {activeSessions.Count()} phiên đang hoạt động");
        /// </example>
        Task<IEnumerable<PhienDangNhap>> GetActiveSessionsAsync(int userId);

        /// <summary>
        /// Tìm phiên đăng nhập theo token (dùng để xác thực JWT/Bearer token).
        /// </summary>
        /// <param name="token">Token cần tìm kiếm.</param>
        /// <returns>Phiên đăng nhập tương ứng hoặc null nếu không tìm thấy.</returns>
        /// <example>
        /// var session = await _repo.GetByTokenAsync("eyJhbGciOiJIUzI1NiIs...");
        /// if (session?.TrangThai == true && session.ThoiGianHetHan > DateTime.Now)
        /// {
        ///     // Token hợp lệ
        /// }
        /// </example>
        Task<PhienDangNhap?> GetByTokenAsync(string token);

        // =====================================================================
        // PHẦN 3: ADMIN - FORCE LOGOUT & QUẢN LÝ BẢO MẬT
        // =====================================================================

        /// <summary>
        /// Vô hiệu hóa tất cả phiên đăng nhập của user (Force Logout).
        /// Admin dùng khi cần đăng xuất user khỏi tất cả thiết bị.
        /// </summary>
        /// <param name="userId">ID của người dùng cần force logout.</param>
        /// <returns>Số lượng phiên đã vô hiệu hóa.</returns>
        /// <example>
        /// int count = await _repo.InvalidateAllUserSessionsAsync(userId: 123);
        /// Console.WriteLine($"Đã đăng xuất {count} phiên của user");
        /// </example>
        Task<int> InvalidateAllUserSessionsAsync(int userId);

        /// <summary>
        /// Vô hiệu hóa một phiên đăng nhập cụ thể theo SessionID.
        /// </summary>
        /// <param name="sessionId">ID của phiên đăng nhập cần vô hiệu hóa.</param>
        /// <returns>True nếu thành công, False nếu không tìm thấy phiên.</returns>
        /// <example>
        /// bool success = await _repo.InvalidateSessionAsync(sessionId: 456);
        /// </example>
        Task<bool> InvalidateSessionAsync(int sessionId);

        // =====================================================================
        // PHẦN 4: MAINTENANCE & CLEANUP
        // =====================================================================

        /// <summary>
        /// Xóa các phiên đăng nhập đã hết hạn (ThoiGianHetHan < DateTime.Now).
        /// Dùng cho scheduled job/background service để dọn dẹp database.
        /// </summary>
        /// <returns>Số lượng phiên đã xóa.</returns>
        /// <example>
        /// // Trong background service hoặc scheduled job
        /// int cleaned = await _repo.CleanupExpiredSessionsAsync();
        /// _logger.LogInformation($"Cleaned up {cleaned} expired sessions");
        /// </example>
        Task<int> CleanupExpiredSessionsAsync();

        /// <summary>
        /// Đếm số phiên đăng nhập đang hoạt động (active) trong hệ thống.
        /// Dùng cho dashboard admin để hiển thị số user online.
        /// </summary>
        /// <returns>Tổng số phiên đang active.</returns>
        /// <example>
        /// int activeCount = await _repo.CountActiveSessionsAsync();
        /// // Hiển thị trên dashboard: "500 users online"
        /// </example>
        Task<int> CountActiveSessionsAsync();

        // =====================================================================
        // PHẦN 5: THỐNG KÊ & BÁO CÁO (CHO ADMIN)
        // =====================================================================

        /// <summary>
        /// Lấy danh sách phiên đăng nhập trong khoảng thời gian.
        /// Dùng cho báo cáo hoạt động user.
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu.</param>
        /// <param name="endDate">Ngày kết thúc.</param>
        /// <returns>Danh sách phiên đăng nhập trong khoảng thời gian.</returns>
        /// <example>
        /// var sessions = await _repo.GetSessionsByDateRangeAsync(
        ///     startDate: DateTime.Now.AddDays(-7),
        ///     endDate: DateTime.Now
        /// );
        /// Console.WriteLine($"Có {sessions.Count()} lượt đăng nhập trong 7 ngày qua");
        /// </example>
        Task<IEnumerable<PhienDangNhap>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Đếm số lượt đăng nhập trong khoảng thời gian (cho thống kê).
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu.</param>
        /// <param name="endDate">Ngày kết thúc.</param>
        /// <returns>Tổng số lượt đăng nhập.</returns>
        /// <example>
        /// int loginCount = await _repo.CountLoginsByDateRangeAsync(
        ///     startDate: DateTime.Today,
        ///     endDate: DateTime.Now
        /// );
        /// Console.WriteLine($"{loginCount} logins today");
        /// </example>
        Task<int> CountLoginsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}