// Models/Interfaces/IContentManagementService.cs
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IContentManagementService
    {
        Task<int> CreateQuestionAsync(QuizCreateEditModel model);
        Task<bool> UpdateQuestionAsync(int questionId, QuizCreateEditModel model);
        Task<bool> DeleteQuestionAsync(int questionId);
        Task<ChuDe> CreateTopicAsync(ChuDe topic);
    }
}