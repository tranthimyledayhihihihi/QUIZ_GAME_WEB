// Models/Interfaces/IProfileService.cs
using QUIZ_GAME_WEB.Models.InputModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IProfileService
    {
        Task<bool> UpdateProfileAsync(int userId, ProfileUpdateModel model);
        Task<bool> UpdateUserSettingAsync(int userId, bool amThanh, bool nhacNen, bool thongBao, string ngonNgu);
    }
}