// Models/Implementations/SystemRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Linq; // Cần cho Where, Select
using System.Threading.Tasks; // 👈 ĐÃ THÊM: Cần cho Task, async/await
using System.Collections.Generic; // 👈 ĐÃ THÊM: Cần cho IEnumerable

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Kế thừa GenericRepository<SystemSetting> và triển khai ISystemRepository
    public class SystemRepository : GenericRepository<SystemSetting>, ISystemRepository
    {
        // Truy cập Context thông qua thuộc tính Context của lớp cơ sở hoặc thuộc tính riêng
        private readonly QuizGameContext _context;

        // Constructor
        public SystemRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy giá trị của một cài đặt hệ thống theo khóa (Key).
        /// </summary>
        public async Task<string?> GetSettingValueByKeyAsync(string key)
        {
            // SỬA: SystemSetting -> SystemSettings
            return await _context.SystemSettings
                                 .Where(s => s.Key == key)
                                 .Select(s => s.Value)
                                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Cập nhật giá trị của một cài đặt hệ thống.
        /// </summary>
        public async Task<bool> UpdateSettingValueAsync(string key, string value)
        {
            // SỬA: SystemSetting -> SystemSettings
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);

            if (setting == null) return false;

            setting.Value = value;

            // SỬA: SystemSetting -> SystemSettings
            _context.SystemSettings.Update(setting);

            // Lưu ý: Cần CompleteAsync() từ UnitOfWork để lưu thay đổi
            return true;
        }

        /// <summary>
        /// Lấy tất cả các cài đặt hệ thống.
        /// </summary>
        public async Task<IEnumerable<SystemSetting>> GetAllSettingsAsync()
        {
            // SỬA: SystemSetting -> SystemSettings
            return await _context.SystemSettings.ToListAsync();
        }
    }
}