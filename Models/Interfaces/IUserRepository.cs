using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ViewModels; // Giả định UserProfileDto tồn tại
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Kế thừa các hàm CRUD cơ bản (ví dụ: Add, Update, Remove) từ IGenericRepository<NguoiDung>
    public interface IUserRepository : IGenericRepository<NguoiDung>
    {
        // ----------------------------------------------------
        // CÁC HÀM TRUY VẤN NGUOI DUNG
        // ----------------------------------------------------
        Task<NguoiDung?> GetByTenDangNhapAsync(string username);
        Task<bool> IsUsernameOrEmailInUseAsync(string username, string email);
        Task<int> CountTotalUsersAsync();
        /// Lấy bản ghi CaiDatNguoiDung theo UserID.
        /// </summary>
        Task<CaiDatNguoiDung?> GetCaiDatByUserIdAsync(int userId);

        /// <summary>
        /// Thêm bản ghi cài đặt mới.
        /// </summary>
        void AddCaiDat(CaiDatNguoiDung setting);

        /// <summary>
        /// Cập nhật bản ghi cài đặt.
        /// </summary>
        void UpdateCaiDat(CaiDatNguoiDung setting);

        // ✅ BỔ SUNG: Lấy Hồ sơ Công khai (Dùng cho SocialController.GetPublicProfile)
        Task<UserProfileDto?> GetPublicProfileAsync(int targetUserId);
        Task<bool> IsUserExistsAsync(int userId);
        // ----------------------------------------------------
        // CÁC HÀM LIÊN QUAN ĐẾN ROLE & ADMIN
        // ----------------------------------------------------
        Task<VaiTro?> GetRoleByUserIdAsync(int userId);
        Task<VaiTro?> GetRoleByIdAsync(int roleId);
        Task<Admin?> GetAdminEntryByUserIdAsync(int userId);
        void AddAdminEntry(Admin entry);

        // ----------------------------------------------------
        // CÁC HÀM TRUY VẤN PHỨC HỢP
        // ----------------------------------------------------
        Task<NguoiDung?> GetUserWithSettingsAndAdminInfoAsync(int userId);
        void Update(Admin adminEntry);
        Task<bool> IsEmailInUseAsync(string email);
    }
}