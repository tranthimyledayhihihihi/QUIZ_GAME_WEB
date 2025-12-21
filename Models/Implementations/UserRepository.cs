using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels; // Cần cho UserProfileDto
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Giả định GenericRepository là lớp cơ sở bạn đã tạo
    public class UserRepository : GenericRepository<NguoiDung>, IUserRepository
    {
        // ✅ Khắc phục lỗi "hides inherited member": Sử dụng 'new'
        private new readonly QuizGameContext _context;

        public UserRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // ==========================================================
        // ✅ OVERRIDE GetByIdAsync ĐỂ KHẮC PHỤC LỖI NullReferenceException
        // Hàm này được dùng trong QLNguoiDungController.GetUserDetails
        // ==========================================================
        public override async Task<NguoiDung?> GetByIdAsync(int id)
        {
            // Eager Load (Include) VaiTro để Controller có thể truy cập user.VaiTro.TenVaiTro
            return await _context.NguoiDungs
                                 .Include(u => u.VaiTro)
                                 .FirstOrDefaultAsync(u => u.UserID == id);
        }

        // ----------------------------------------------------
        // I. CÁC HÀM TRUY VẤN NGUOIDUNG CƠ BẢN
        // ----------------------------------------------------

        public async Task<NguoiDung?> GetByTenDangNhapAsync(string username)
        {
            return await _context.NguoiDungs
                                 .FirstOrDefaultAsync(u => u.TenDangNhap == username);
        }

        public async Task<bool> IsUsernameOrEmailInUseAsync(string username, string email)
        {
            return await _context.NguoiDungs
                                 .AnyAsync(u => u.TenDangNhap == username || u.Email == email);
        }

        public async Task<int> CountTotalUsersAsync()
        {
            return await _context.NguoiDungs.CountAsync();
        }

        public async Task<bool> IsEmailInUseAsync(string email)
        {
            return await _context.NguoiDungs
                                 .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUserExistsAsync(int userId)
        {
            return await _context.NguoiDungs.AnyAsync(u => u.UserID == userId);
        }

        // ----------------------------------------------------
        // II. CÁC HÀM LIÊN QUAN ĐẾN ROLE & ADMIN
        // ----------------------------------------------------

        public void AddAdminEntry(Admin entry)
        {
            _context.Admins.Add(entry);
        }

        public async Task<Admin?> GetAdminEntryByUserIdAsync(int userId)
        {
            return await _context.Admins
                                 .FirstOrDefaultAsync(a => a.UserID == userId);
        }

        // Hàm này không cần thiết vì đã có RoleRepository, nhưng giữ lại nếu IUserRepository yêu cầu
        public async Task<VaiTro?> GetRoleByIdAsync(int roleId)
        {
            return await _context.VaiTros.FindAsync(roleId);
        }

        public async Task<VaiTro?> GetRoleByUserIdAsync(int userId)
        {
            // Giả định VaiTroID được lưu trong bảng NguoiDung (u.VaiTroID)
            var role = await (from u in _context.NguoiDungs
                              join r in _context.VaiTros on u.VaiTroID equals r.VaiTroID
                              where u.UserID == userId
                              select r)
                              .FirstOrDefaultAsync();
            return role;
        }

        public void Update(Admin adminEntry)
        {
            _context.Admins.Update(adminEntry);
        }

        // ----------------------------------------------------
        // III. CÁC HÀM TRUY VẤN PHỨC HỢP & SOCIAL
        // ----------------------------------------------------

        public async Task<NguoiDung?> GetUserWithSettingsAndAdminInfoAsync(int userId)
        {
            return await _context.NguoiDungs
                                 .Include(u => u.CaiDat)
                                 .Include(u => u.Admin)
                                 .Include(u => u.VaiTro) // Đảm bảo Role được tải
                                 .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<CaiDatNguoiDung?> GetCaiDatByUserIdAsync(int userId)
        {
            return await _context.CaiDatNguoiDungs
                                 .FirstOrDefaultAsync(c => c.UserID == userId);
        }

        public void AddCaiDat(CaiDatNguoiDung setting)
        {
            _context.CaiDatNguoiDungs.Add(setting);
        }

        public void UpdateCaiDat(CaiDatNguoiDung setting)
        {
            _context.CaiDatNguoiDungs.Update(setting);
        }

        public async Task<UserProfileDto?> GetPublicProfileAsync(int targetUserId)
        {
            // Tải các Navigation Properties cần thiết để tính toán
            var userQuery = _context.NguoiDungs
                .Include(u => u.BXHs)
                .Include(u => u.KetQuas)
                .AsNoTracking()
                .Where(u => u.UserID == targetUserId);

            var profile = await userQuery
                .Select(u => new UserProfileDto
                {
                    UserID = u.UserID,
                    TenDangNhap = u.TenDangNhap,
                    HoTen = u.HoTen,
                    AnhDaiDien = u.AnhDaiDien,
                    NgayDangKy = u.NgayDangKy,

                    // Tính toán các chỉ số thống kê cơ bản:
                    // Lấy điểm cao nhất trong BXH (giả định BXH chỉ có 1 bản ghi cho mỗi user hoặc lấy max)
                    TongSoDiem = u.BXHs.OrderByDescending(b => b.DiemThang).Select(b => b.DiemThang).FirstOrDefault(),
                    TongSoQuizDaLam = u.KetQuas.Count(),

                    // Placeholder cho Social
                    IsFollowing = false
                })
                .FirstOrDefaultAsync();

            return profile;
        }
    }
}