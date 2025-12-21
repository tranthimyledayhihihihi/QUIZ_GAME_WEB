// Models/Interfaces/IAdminDashboardService.cs
using QUIZ_GAME_WEB.Models.ViewModels; // <-- ĐÃ SỬA: Dùng ViewModels

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardModel> GetDashboardSummaryAsync();
        Task<ThongKeViewModel> GetUserActivityStatsAsync();
    }
}