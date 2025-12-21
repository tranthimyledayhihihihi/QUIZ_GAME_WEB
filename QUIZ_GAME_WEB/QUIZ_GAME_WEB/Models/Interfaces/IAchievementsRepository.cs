using QUIZ_GAME_WEB.Models.ResultsModels; // Cần cho Entity ThanhTuu
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Kế thừa các hàm CRUD cơ bản từ IGenericRepository<ThanhTuu>
    public interface IAchievementsRepository : IGenericRepository<ThanhTuu>
    {
        /// <summary>
        /// Lấy tất cả các thành tựu mà một người dùng đã đạt được.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <returns>Danh sách các ThanhTuu mà người dùng đó đã có.</returns>
        Task<IEnumerable<ThanhTuu>> GetAchievementsByUserIdAsync(int userId);

        /// <summary>
        /// Kiểm tra xem người dùng đã đạt được một thành tựu cụ thể hay chưa.
        /// </summary>
        /// <param name="userId">ID của người dùng.</param>
        /// <param name="achievementCode">Mã code của thành tựu (ví dụ: "FIRST_QUIZ").</param>
        /// <returns>True nếu đã đạt được, ngược lại là False.</returns>
        Task<bool> HasUserAchievedAsync(int userId, string achievementCode);

        /// <summary>
        /// Lấy tất cả các thành tựu có sẵn trong game (dùng cho danh sách tổng quan).
        /// </summary>
        Task<IEnumerable<ThanhTuu>> GetAllAvailableAchievementsAsync();
    }
}