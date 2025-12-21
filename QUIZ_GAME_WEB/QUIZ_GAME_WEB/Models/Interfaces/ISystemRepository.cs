// Models/Interfaces/ISystemRepository.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface ISystemRepository : IGenericRepository<SystemSetting>
    {
        // Lấy giá trị cài đặt theo Key (dùng cho Service đọc cấu hình)
        Task<string?> GetSettingValueByKeyAsync(string key);

        // Cập nhật giá trị cài đặt theo Key (dùng cho Admin Service)
        Task<bool> UpdateSettingValueAsync(string key, string value);

        Task<IEnumerable<SystemSetting>> GetAllSettingsAsync();
    }
}