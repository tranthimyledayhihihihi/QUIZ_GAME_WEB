// =====================================================================
// File: Models/Implementations/LoginSessionRepository.cs
// Mục đích: Repository triển khai quản lý phiên đăng nhập
// Tác giả: QUIZ_GAME_WEB
// Ngày tạo: 2025
// =====================================================================

using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    /// <summary>
    /// Repository triển khai quản lý phiên đăng nhập - LoginSessionRepository.
    /// Kế thừa GenericRepository<PhienDangNhap> và triển khai ILoginSessionRepository.
    /// </summary>
    public class LoginSessionRepository : GenericRepository<PhienDangNhap>, ILoginSessionRepository
    {
        // =====================================================================
        // FIELDS & CONSTRUCTOR
        // =====================================================================

        /// <summary>
        /// DbContext để truy cập database.
        /// </summary>
        private readonly QuizGameContext _context;

        /// <summary>
        /// Constructor - Dependency Injection QuizGameContext.
        /// </summary>
        /// <param name="context">Database context được inject từ DI container.</param>
        public LoginSessionRepository(QuizGameContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // =====================================================================
        // PHẦN 1: QUẢN LÝ LỊCH SỬ ĐĂNG NHẬP
        // =====================================================================

        /// <summary>
        /// Lấy lịch sử đăng nhập gần nhất của user.
        /// Sắp xếp theo ThoiGianDangNhap giảm dần (mới nhất trước).
        /// </summary>
        public async Task<IEnumerable<PhienDangNhap>> GetLoginHistoryAsync(int userId, int limit = 20)
        {
            try
            {
                return await _context.PhienDangNhaps
                    .Where(p => p.UserID == userId)
                    .OrderByDescending(p => p.ThoiGianBatDau)
                    .Take(limit)
                    .AsNoTracking() // Tăng performance cho read-only query
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                throw new Exception($"Lỗi khi lấy lịch sử đăng nhập của user {userId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy phiên đăng nhập gần nhất của user.
        /// </summary>
        public async Task<PhienDangNhap?> GetLastLoginSessionAsync(int userId)
        {
            try
            {
                return await _context.PhienDangNhaps
                    .Where(p => p.UserID == userId)
                    .OrderByDescending(p => p.ThoiGianBatDau)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy phiên đăng nhập cuối của user {userId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đếm tổng số lần user đã đăng nhập.
        /// </summary>
        public async Task<int> CountUserLoginAttemptsAsync(int userId)
        {
            try
            {
                return await _context.PhienDangNhaps
                    .CountAsync(p => p.UserID == userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm số lần đăng nhập của user {userId}: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // PHẦN 2: QUẢN LÝ PHIÊN ĐĂNG NHẬP HIỆN TẠI
        // =====================================================================

        /// <summary>
        /// Lấy tất cả phiên đăng nhập đang active (còn hiệu lực).
        /// Điều kiện: TrangThai = true VÀ ThoiGianHetHan > DateTime.Now
        /// </summary>
        public async Task<IEnumerable<PhienDangNhap>> GetActiveSessionsAsync(int userId)
        {
            try
            {
                var now = DateTime.Now;
                return await _context.PhienDangNhaps
                    .Where(p => p.UserID == userId
                        && p.TrangThai == true
                        && p.ThoiGianKetThuc > now)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy phiên active của user {userId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm phiên đăng nhập theo token (JWT/Bearer token).
        /// </summary>
        public async Task<PhienDangNhap?> GetByTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                return await _context.PhienDangNhaps
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Token == token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm phiên theo token: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // PHẦN 3: ADMIN - FORCE LOGOUT & QUẢN LÝ BẢO MẬT
        // =====================================================================

        /// <summary>
        /// Vô hiệu hóa TẤT CẢ phiên đăng nhập của user (Force Logout).
        /// Dùng khi admin muốn đăng xuất user khỏi tất cả thiết bị.
        /// </summary>
        public async Task<int> InvalidateAllUserSessionsAsync(int userId)
        {
            try
            {
                // Lấy tất cả phiên đang active
                var activeSessions = await _context.PhienDangNhaps
                    .Where(p => p.UserID == userId && p.TrangThai == true)
                    .ToListAsync();

                if (!activeSessions.Any())
                    return 0;

                // Vô hiệu hóa tất cả
                foreach (var session in activeSessions)
                {
                    session.TrangThai = false;
                }

                // Lưu thay đổi
                await _context.SaveChangesAsync();
                return activeSessions.Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi vô hiệu hóa phiên của user {userId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Vô hiệu hóa một phiên đăng nhập cụ thể theo SessionID.
        /// </summary>
        public async Task<bool> InvalidateSessionAsync(int sessionId)
        {
            try
            {
                var session = await _context.PhienDangNhaps
                    .FirstOrDefaultAsync(p => p.SessionID == sessionId);

                if (session == null)
                    return false;

                session.TrangThai = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi vô hiệu hóa phiên {sessionId}: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // PHẦN 4: MAINTENANCE & CLEANUP
        // =====================================================================

        /// <summary>
        /// Xóa các phiên đăng nhập đã hết hạn.
        /// Nên chạy định kỳ qua background job/scheduled task.
        /// </summary>
        public async Task<int> CleanupExpiredSessionsAsync()
        {
            try
            {
                var now = DateTime.Now;

                // Tìm các phiên đã hết hạn
                var expiredSessions = await _context.PhienDangNhaps
                    .Where(p => p.ThoiGianKetThuc < now)
                    .ToListAsync();

                if (!expiredSessions.Any())
                    return 0;

                // Xóa tất cả phiên hết hạn
                _context.PhienDangNhaps.RemoveRange(expiredSessions);
                await _context.SaveChangesAsync();

                return expiredSessions.Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cleanup phiên hết hạn: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đếm số phiên đang hoạt động trong toàn hệ thống.
        /// Dùng cho dashboard admin (hiển thị số users online).
        /// </summary>
        public async Task<int> CountActiveSessionsAsync()
        {
            try
            {
                var now = DateTime.Now;
                return await _context.PhienDangNhaps
                    .CountAsync(p => p.TrangThai == true && p.ThoiGianKetThuc > now);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm phiên active: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // PHẦN 5: THỐNG KÊ & BÁO CÁO (CHO ADMIN)
        // =====================================================================

        /// <summary>
        /// Lấy danh sách phiên đăng nhập trong khoảng thời gian.
        /// Dùng cho báo cáo hoạt động user.
        /// </summary>
        public async Task<IEnumerable<PhienDangNhap>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.PhienDangNhaps
                    .Where(p => p.ThoiGianBatDau >= startDate && p.ThoiGianBatDau <= endDate)
                    .OrderByDescending(p => p.ThoiGianBatDau)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy phiên theo khoảng thời gian: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đếm số lượt đăng nhập trong khoảng thời gian.
        /// Dùng cho thống kê dashboard.
        /// </summary>
        public async Task<int> CountLoginsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.PhienDangNhaps
                    .CountAsync(p => p.ThoiGianBatDau >= startDate && p.ThoiGianBatDau <= endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm số login theo thời gian: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // PHẦN 6: HELPER METHODS (PRIVATE - NỘI BỘ)
        // =====================================================================

        /// <summary>
        /// Kiểm tra xem một phiên có còn hiệu lực hay không.
        /// </summary>
        /// <param name="session">Phiên cần kiểm tra.</param>
        /// <returns>True nếu phiên còn hiệu lực.</returns>
        private bool IsSessionValid(PhienDangNhap session)
        {
            if (session == null)
                return false;

            return session.TrangThai == true
                && session.ThoiGianKetThuc.HasValue
                && session.ThoiGianKetThuc.Value > DateTime.Now;
        }
    }
}