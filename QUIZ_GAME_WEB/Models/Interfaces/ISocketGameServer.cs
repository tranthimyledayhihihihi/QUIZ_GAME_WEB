using System.Net.WebSockets;
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

        void Register(int userId, WebSocket socket);

        void Unregister(int userId);

        /* =========================
           MATCHMAKING – RANDOM
        ========================== */
        Task HandleFindRandomMatchAsync(int userId);

        /* =========================
           🔥 PRIVATE ROOM
        ========================== */

        /// <summary>
        /// Tạo phòng riêng – trả về roomCode cho client
        /// </summary>
        Task HandleCreateRoomAsync(int userId);

        /// <summary>
        /// Join phòng riêng bằng mã phòng
        /// </summary>
        Task HandleJoinPrivateRoomAsync(int userId, string roomCode);

        /* =========================
           MATCH ROOM
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