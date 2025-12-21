using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace QUIZ_GAME_WEB.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MatchmakingHub : Hub
    {
        private readonly IMatchmakingQueueService _queue;

        public MatchmakingHub(IMatchmakingQueueService queue)
        {
            _queue = queue;
        }

        private int GetUserId()
        {
            var claim = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim == null) throw new Exception("Token không có NameIdentifier");
            return int.Parse(claim);
        }

        // ============================================
        // 🟦 1) THAM GIA HÀNG ĐỢI NGẪU NHIÊN
        // ============================================
        public async Task JoinQueue()
        {
            int userId = GetUserId();

            await _queue.AddToQueueAsync(userId);

            await Clients.Caller.SendAsync("QueueStatus",
                $"Bạn ({userId}) đã vào hàng đợi!");

            // Hệ thống sẽ tự tìm trận nếu >= 2 người
            await _queue.CheckRandomMatchAsync();
        }

        // ============================================
        // 🟦 2) TẠO PHÒNG RIÊNG
        // ============================================
        public async Task CreateRoom()
        {
            int userId = GetUserId();

            string code = await _queue.CreatePrivateRoomAsync(userId);

            // FE đang lắng nghe "RoomCreated"
            await Clients.Caller.SendAsync("RoomCreated", new
            {
                matchCode = code   // TÊN FE ĐANG MONG CHỜ
            });
        }

        // ============================================
        // 🟦 3) THAM GIA PHÒNG RIÊNG
        // ============================================
        public async Task JoinRoom(string roomCode)
        {
            int userId = GetUserId();

            try
            {
                var (matchCode, creatorId) = await _queue.JoinPrivateRoomAsync(roomCode, userId);

                // FE mong đợi "MatchFound" chứ không phải "RoomJoined"
                await Clients.Caller.SendAsync("MatchFound", new
                {
                    matchCode = matchCode,
                    OpponentId = creatorId
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("JoinFailed", ex.Message);
            }
        }
    }
}
