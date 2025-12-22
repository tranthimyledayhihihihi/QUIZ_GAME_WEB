using System.Threading.Tasks;
using QUIZ_GAME_WEB.Models.InputModels;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface ISocketGameServer
    {
        /* =========================
           CONNECTION
        ========================== */
        Task Handle(HttpContext context);

        /* =========================
           MATCHMAKING – RANDOM
        ========================== */
        Task HandleFindRandomMatchAsync(int userId);

        /* =========================
           🔥 PRIVATE ROOM
        ========================== */
        Task HandleCreateRoomAsync(int userId);

        Task HandleJoinPrivateRoomAsync(int userId, string roomCode);

        /* =========================
           GAMEPLAY
        ========================== */
        Task HandleSubmitAnswerAsync(
            int userId,
            string matchCode,
            MatchAnswerModel answer
        );

        /* =========================
           SEND MESSAGE
        ========================== */
        Task Send(int userId, object message);

        Task Broadcast(string matchCode, object message);

        /* =========================
           INFO
        ========================== */
        int GetOnlineCount();
    }
}
