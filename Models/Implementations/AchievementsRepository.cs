// Models/Implementations/AchievementsRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ResultsModels; // Cần cho Entity ThanhTuu
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    // Giả định GenericRepository là lớp cơ sở và ThanhTuu là Entity chính
    public class AchievementsRepository : GenericRepository<ThanhTuu>, IAchievementsRepository
    {
        private new readonly QuizGameContext _context;

        public AchievementsRepository(QuizGameContext context) : base(context)
        {
            _context = context; // Gán Context để truy cập các DbSet khác nếu cần
        }

        // ------------------------------------------------------------------
        // TRIỂN KHAI GET ACHIEVEMENTS BY USER ID
        // ------------------------------------------------------------------
        public async Task<IEnumerable<ThanhTuu>> GetAchievementsByUserIdAsync(int userId)
        {
            // SỬA LỖI: Thay UserID bằng NguoiDungID (Giả định tên khóa ngoại đúng)
            return await _context.ThanhTuus
                .Where(a => a.NguoiDungID == userId) // 👈 ĐÃ SỬA LỖI CS1061
                .ToListAsync();
        }

        // ------------------------------------------------------------------
        // TRIỂN KHAI CÁC HÀM CÒN LẠI (Nếu có trong IAchievementsRepository)
        // ------------------------------------------------------------------

        public async Task<bool> HasUserAchievedAsync(int userId, string achievementCode)
        {
            // Logic: Kiểm tra xem có bất kỳ bản ghi ThanhTuu nào khớp với UserID và Mã thành tựu không
            // Giả định ThanhTuu có thuộc tính AchievementCode
            return await _context.ThanhTuus
                .AnyAsync(a => a.NguoiDungID == userId && a.AchievementCode == achievementCode);
        }

        public async Task<IEnumerable<ThanhTuu>> GetAllAvailableAchievementsAsync()
        {
            // Logic: Lấy tất cả các thành tựu (thường là các định nghĩa master data)
            // Tùy thuộc vào thiết kế: Nếu ThanhTuu là master data, chỉ cần trả về tất cả.
            // Nếu ThanhTuu chỉ là bản ghi user đạt được, hàm này cần một Entity MasterData khác.
            // Giả định ThanhTuu có MasterDataID=0 hoặc có cờ IsMasterData
            return await _context.ThanhTuus
                // .Where(a => a.IsMasterData) // Nếu có cờ IsMasterData
                .ToListAsync();
        }
    }
}