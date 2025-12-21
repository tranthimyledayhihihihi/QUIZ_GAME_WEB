// Models/Interfaces/IUserService.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IUserService
    {
        Task<NguoiDung?> GetUserByIdAsync(int userId);
        Task<string> GetUserRoleAsync(int userId);
    }
}