// Models/Interfaces/IRewardService.cs
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IRewardService
    {
        Task<IEnumerable<ThanhTuu>> GetUserAchievementsAsync(int userId);
        Task<bool> CheckAndAwardDailyRewardAsync(int userId);
    }
}