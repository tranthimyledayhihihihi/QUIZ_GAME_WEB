using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.CoreEntities; // Cần cho NguoiDung, CaiDatNguoiDung
using System;
using System.Threading.Tasks;
using System.Security.Cryptography; // Để giả lập Hash

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Giả định bạn đã đăng ký ProfileService là triển khai của IProfileService
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===============================================
        // HÀM HỖ TRỢ (SECURITY)
        // ===============================================

        // Trong môi trường thực tế, sử dụng thư viện mạnh như BCrypt.Net
        private string HashPassword(string password)
        {
            return $"hashed_{password}_password";
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return hashedPassword == HashPassword(inputPassword);
        }


        // ===============================================
        // 1. CẬP NHẬT HỒ SƠ (ProfileUpdateModel)
        // ===============================================

        public async Task<bool> UpdateProfileAsync(int userId, ProfileUpdateModel model)
        {
            // Tải người dùng kèm theo cài đặt (nếu cần) và Admin Info
            var user = await _unitOfWork.Users.GetUserWithSettingsAndAdminInfoAsync(userId);

            if (user == null) return false;

            // --- 1. Xử lý thay đổi Email ---
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                // Kiểm tra Email đã được sử dụng bởi người khác chưa
                if (await _unitOfWork.Users.IsEmailInUseAsync(model.Email))
                {
                    // Trong trường hợp này, ta có thể throw Exception hoặc trả về false
                    throw new Exception("Email đã được sử dụng bởi tài khoản khác.");
                }
                user.Email = model.Email;
            }

            // --- 2. Xử lý thay đổi Mật khẩu ---
            if (!string.IsNullOrEmpty(model.MatKhauMoi))
            {
                // BẮT BUỘC: Kiểm tra MatKhauHienTai phải được cung cấp để xác thực
                if (string.IsNullOrEmpty(model.MatKhauHienTai))
                {
                    throw new Exception("Vui lòng cung cấp mật khẩu hiện tại để thay đổi mật khẩu.");
                }

                if (!VerifyPassword(model.MatKhauHienTai, user.MatKhau))
                {
                    throw new Exception("Mật khẩu hiện tại không đúng.");
                }

                // Hash và cập nhật mật khẩu mới
                user.MatKhau = HashPassword(model.MatKhauMoi);
            }

            // --- 3. Cập nhật các trường khác ---
            if (!string.IsNullOrEmpty(model.HoTen)) user.HoTen = model.HoTen;
            if (!string.IsNullOrEmpty(model.AnhDaiDien)) user.AnhDaiDien = model.AnhDaiDien;

            // Cập nhật Entity trong Context
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ===============================================
        // 2. CẬP NHẬT CÀI ĐẶT NGƯỜI DÙNG (Settings)
        // ===============================================

        public async Task<bool> UpdateUserSettingAsync(
            int userId,
            bool amThanh,
            bool nhacNen,
            bool thongBao,
            string ngonNgu)
        {
            // Tải cài đặt người dùng (CaiDatNguoiDung)
            // Giả định User Entity đã được tải kèm theo CaiDat, 
            // hoặc ta dùng hàm truy vấn trực tiếp CaiDatNguoiDung

            var settings = await _unitOfWork.Users.GetCaiDatByUserIdAsync(userId); // Giả định hàm này tồn tại

            if (settings == null)
            {
                // Nếu cài đặt không tồn tại, tạo mới
                var newSettings = new CaiDatNguoiDung
                {
                    UserID = userId,
                    AmThanh = amThanh,
                    NhacNen = nhacNen,
                    ThongBao = thongBao,
                    NgonNgu = ngonNgu
                };

                _unitOfWork.Users.AddCaiDat(newSettings); // Giả định hàm AddCaiDat tồn tại
            }
            else
            {
                // Cập nhật các giá trị hiện có
                settings.AmThanh = amThanh;
                settings.NhacNen = nhacNen;
                settings.ThongBao = thongBao;
                settings.NgonNgu = ngonNgu; 

                _unitOfWork.Users.UpdateCaiDat(settings); // Giả định hàm UpdateCaiDat tồn tại
            }

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}