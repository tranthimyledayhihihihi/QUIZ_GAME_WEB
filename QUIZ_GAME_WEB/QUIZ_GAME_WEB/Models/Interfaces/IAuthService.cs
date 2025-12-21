// Models/Interfaces/IAuthService.cs (Đã sửa)
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels; // <-- Dùng ViewModels

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IAuthService
    {
        // Thay đổi kiểu trả về thành LoginResponseModel
        Task<LoginResponseModel?> DangNhapAsync(DangNhapModel model);
        Task<bool> DangKyAsync(DangKyModel model);
        // ...
    }
}