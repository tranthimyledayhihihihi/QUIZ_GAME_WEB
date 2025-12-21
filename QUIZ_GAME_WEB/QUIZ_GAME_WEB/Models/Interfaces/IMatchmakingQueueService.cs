// Models/Interfaces/IMatchmakingQueueService.cs

using System;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IMatchmakingQueueService
    {
        Task AddToQueueAsync(int userId);
        Task RemoveFromQueueAsync(int userId);

        Task CheckRandomMatchAsync();
        Task<string> CreatePrivateRoomAsync(int creatorId);
        Task<(string matchCode, int creatorId)> JoinPrivateRoomAsync(string roomCode, int joinerId);
    }


}