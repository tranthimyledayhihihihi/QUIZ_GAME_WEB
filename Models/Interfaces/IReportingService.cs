// Models/Interfaces/IReportingService.cs
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.ViewModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IReportingService
    {
        Task<BaoCaoModel> GenerateReportAsync(BaoCaoRequestModel request);
        Task<ThongKeViewModel> GetUserPerformanceStatsAsync(int? chuDeId);
    }
}