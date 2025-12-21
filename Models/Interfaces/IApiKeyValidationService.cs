// Models/Interfaces/IApiKeyValidationService.cs
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IApiKeyValidationService
    {
        Task<bool> IsValidApiKey(string userApiKey);
    }
}