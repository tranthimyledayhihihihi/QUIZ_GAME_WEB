// =====================================================================
// File: Models/Implementations/UserManagementService.cs
// Mục đích: Service quản lý người dùng cho Admin
// Đã sửa lỗi: Thay GetByConditionAsync bằng GetLoginHistoryAsync
// =====================================================================

using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    /// <summary>
    /// Service quản lý người dùng cho Admin Dashboard.
    /// Xử lý các nghiệp vụ: xem danh sách user, khóa/mở khóa, cập nhật role, v.v.
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor - Inject UnitOfWork để truy cập repositories.
        /// </summary>
        public UserManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // =====================================================================
        // 1. LẤY DANH SÁCH NGƯỜI DÙNG CHO ADMIN DASHBOARD
        // =====================================================================
        /// <summary>
        /// Lấy danh sách tất cả người dùng kèm vai trò để hiển thị trên Admin Dashboard.
        /// </summary>
        /// <returns>Danh sách NguoiDungAdminViewModel.</returns>
        public async Task<IEnumerable<NguoiDungAdminViewModel>> GetAllUsersForAdminAsync()
        {
            try
            {
                // Bước 1: Lấy danh sách người dùng từ repository
                var users = await _unitOfWork.Users.GetAllAsync();

                // Bước 2: Chuyển đổi sang ViewModel kèm thông tin vai trò
                var userViewModels = new List<NguoiDungAdminViewModel>();

                foreach (var user in users)
                {
                    // Lấy vai trò của user (nếu có)
                    var role = await _unitOfWork.Users.GetRoleByUserIdAsync(user.UserID);
                    string roleName = role?.TenVaiTro ?? "Player"; // Mặc định là Player

                    userViewModels.Add(new NguoiDungAdminViewModel
                    {
                        UserID = user.UserID,
                        TenDangNhap = user.TenDangNhap,
                        HoTen = user.HoTen,
                        Email = user.Email,
                        VaiTro = roleName,
                        NgayDangKy = user.NgayDangKy,
                        LanDangNhapCuoi = user.LanDangNhapCuoi,
                        TrangThai = user.TrangThai
                    });
                }

                return userViewModels;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách người dùng: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 2. LẤY CHI TIẾT NGƯỜI DÙNG
        // =====================================================================
        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng theo UserID.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy.</returns>
        public async Task<NguoiDung?> GetUserDetailByIdAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết user {userId}: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 3. LẤY LỊCH SỬ ĐĂNG NHẬP - ĐÃ SỬA LỖI
        // =====================================================================
        /// <summary>
        /// Lấy lịch sử đăng nhập gần nhất của người dùng.
        /// ✅ ĐÃ SỬA: Thay GetByConditionAsync bằng GetLoginHistoryAsync
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="limit">Số lượng bản ghi tối đa (mặc định 10).</param>
        /// <returns>Danh sách phiên đăng nhập.</returns>
        public async Task<IEnumerable<PhienDangNhap>> GetUserLoginHistoryAsync(int userId, int limit = 10)
        {
            try
            {
                // ✅ SỬA LỖI: Dùng method GetLoginHistoryAsync đã định nghĩa trong ILoginSessionRepository
                var history = await _unitOfWork.LoginSessions.GetLoginHistoryAsync(userId, limit);
                return history;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lịch sử đăng nhập của user {userId}: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 4. KHÓA/MỞ KHÓA NGƯỜI DÙNG (TOGGLE LOCKOUT)
        // =====================================================================
        /// <summary>
        /// Khóa hoặc mở khóa tài khoản người dùng.
        /// Logic: Không cho phép khóa SuperAdmin.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="isLocked">True = Khóa, False = Mở khóa.</param>
        /// <returns>True nếu thành công, False nếu thất bại.</returns>
        public async Task<bool> ToggleUserLockoutAsync(int userId, bool isLocked)
        {
            try
            {
                // Bước 1: Lấy thông tin user
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return false;

                // Bước 2: Kiểm tra vai trò - KHÔNG CHO PHÉP KHÓA SUPERADMIN
                var role = await _unitOfWork.Users.GetRoleByUserIdAsync(userId);
                if (role != null && role.TenVaiTro == "SuperAdmin")
                {
                    // Không được khóa SuperAdmin
                    return false;
                }

                // Bước 3: Cập nhật trạng thái
                user.TrangThai = !isLocked; // True = Active, False = Locked

                // Bước 4: Lưu thay đổi
                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi khóa/mở khóa user {userId}: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 5. CẬP NHẬT VAI TRÒ NGƯỜI DÙNG (UPDATE ROLE)
        // =====================================================================
        /// <summary>
        /// Cập nhật vai trò (Role) của người dùng.
        /// Logic: Tạo entry Admin mới nếu chưa có, hoặc cập nhật nếu đã có.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="newRoleId">ID vai trò mới.</param>
        /// <returns>True nếu thành công, False nếu thất bại.</returns>
        public async Task<bool> UpdateUserRoleAsync(int userId, int newRoleId)
        {
            try
            {
                // Bước 1: Kiểm tra User và Role hợp lệ
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var role = await _unitOfWork.Users.GetRoleByIdAsync(newRoleId);

                if (user == null || role == null)
                    return false;

                // Bước 2: Tìm hoặc tạo entry trong bảng Admin
                var adminEntry = await _unitOfWork.Users.GetAdminEntryByUserIdAsync(userId);

                if (adminEntry == null)
                {
                    // Tạo entry Admin mới nếu user chưa có vai trò admin
                    _unitOfWork.Users.AddAdminEntry(new Admin
                    {
                        UserID = userId,
                        VaiTroID = newRoleId,
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    });
                }
                else
                {
                    // Cập nhật Role nếu user đã là admin
                    adminEntry.VaiTroID = newRoleId;
                    // ✅ KHÔNG CẦN GỌI Update() - EF Core tự tracking
                }

                // Bước 3: Lưu thay đổi
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật vai trò cho user {userId}: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 6. FORCE LOGOUT USER (BỔ SUNG THÊM)
        // =====================================================================
        /// <summary>
        /// Đăng xuất người dùng khỏi tất cả thiết bị (vô hiệu hóa tất cả phiên).
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Số lượng phiên đã vô hiệu hóa.</returns>
        public async Task<int> ForceLogoutUserAsync(int userId)
        {
            try
            {
                var count = await _unitOfWork.LoginSessions.InvalidateAllUserSessionsAsync(userId);
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi force logout user {userId}: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 7. ĐẾM TỔNG SỐ NGƯỜI DÙNG (CHO DASHBOARD)
        // =====================================================================
        /// <summary>
        /// Đếm tổng số người dùng trong hệ thống.
        /// </summary>
        /// <returns>Tổng số user.</returns>
        public async Task<int> GetTotalUsersCountAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                return users.Count();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm tổng số user: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 8. ĐẾM SỐ USER ĐANG ONLINE (CHO DASHBOARD)
        // =====================================================================
        /// <summary>
        /// Đếm số người dùng đang online (có phiên active).
        /// </summary>
        /// <returns>Số user online.</returns>
        public async Task<int> GetOnlineUsersCountAsync()
        {
            try
            {
                var count = await _unitOfWork.LoginSessions.CountActiveSessionsAsync();
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đếm user online: {ex.Message}", ex);
            }
        }

        // =====================================================================
        // 9. LẤY DANH SÁCH USER MỚI ĐĂNG KÝ (CHO DASHBOARD)
        // =====================================================================
        /// <summary>
        /// Lấy danh sách người dùng mới đăng ký gần đây.
        /// </summary>
        /// <param name="limit">Số lượng user cần lấy.</param>
        /// <returns>Danh sách user mới.</returns>
        public async Task<IEnumerable<NguoiDung>> GetRecentlyRegisteredUsersAsync(int limit = 10)
        {
            try
            {
                var allUsers = await _unitOfWork.Users.GetAllAsync();
                return allUsers
                    .OrderByDescending(u => u.NgayDangKy)
                    .Take(limit)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách user mới: {ex.Message}", ex);
            }
        }
    }
}