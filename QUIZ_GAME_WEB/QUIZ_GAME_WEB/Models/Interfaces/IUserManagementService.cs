// Models/Interfaces/IUserManagementService.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Đặt trong thư mục Interfaces/AdminServices (hoặc Interfaces nếu bạn không dùng thư mục con)
    public interface IUserManagementService
    {
        // Quản lý người dùng
        Task<IEnumerable<NguoiDungAdminViewModel>> GetAllUsersForAdminAsync();
        Task<NguoiDung?> GetUserDetailByIdAsync(int userId);

        // Cập nhật trạng thái
        Task<bool> ToggleUserLockoutAsync(int userId, bool isLocked);
        Task<bool> UpdateUserRoleAsync(int userId, int newRoleId);

        // Lịch sử đăng nhập/Hoạt động
        Task<IEnumerable<PhienDangNhap>> GetUserLoginHistoryAsync(int userId, int limit = 10);
    }
}