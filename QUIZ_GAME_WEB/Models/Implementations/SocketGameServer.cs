using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.InputModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class SocketGameServer : ISocketGameServer
    {
        private readonly IServiceProvider _serviceProvider;

        // ===============================
        // SOCKET + MATCH STORAGE
        // ===============================
        private readonly ConcurrentDictionary<int, WebSocket> _userSockets = new();
        private readonly ConcurrentDictionary<string, List<int>> _matchRooms = new();

        // ===============================
        // RANDOM MATCH QUEUE
        // ===============================
        private readonly List<int> _randomQueue = new();
        private readonly object _queueLock = new();

        // ===============================
        // PRIVATE WAITING ROOMS
        // ===============================
        private readonly ConcurrentDictionary<string, List<int>> _waitingRooms = new();

        public SocketGameServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /* =========================================================
           HANDLE WEBSOCKET CONNECTION
        ========================================================= */
        public async Task Handle(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            Register(userId, socket);

            try
            {
                await Listen(userId, socket);
            }
            finally
            {
                Unregister(userId);
                lock (_queueLock) _randomQueue.Remove(userId);
            }
        }

        private async Task Listen(int userId, WebSocket socket)
        {
            var buffer = new byte[4096];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    break;
                }

                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await HandleClientMessage(userId, msg);
            }
        }

        /* =========================================================
           HANDLE CLIENT MESSAGE
        ========================================================= */
        private async Task HandleClientMessage(int userId, string message)
        {
            var data = JsonSerializer.Deserialize<ClientMessage>(message);
            if (data == null) return;

            Console.WriteLine($"📨 Received message from user {userId}: {data.Type}");

            switch (data.Type)
            {
                case "FIND_MATCH":
                    await HandleFindRandomMatchAsync(userId);
                    break;

                case "CANCEL_QUEUE":
                    lock (_queueLock) _randomQueue.Remove(userId);
                    await Send(userId, new { Type = "QUEUE_CANCELLED" });
                    break;

                case "CREATE_ROOM":
                    await HandleCreateRoomAsync(userId);
                    break;

                case "JOIN_ROOM_CODE":
                    var roomCode = data.Data?.GetProperty("roomCode").GetString();
                    if (!string.IsNullOrEmpty(roomCode))
                        await HandleJoinPrivateRoomAsync(userId, roomCode);
                    break;

                // 🔥 FIX: THÊM HANDLER CHO JOIN_MATCH
                case "JOIN_MATCH":
                    var matchCode = data.Data?.GetProperty("matchCode").GetString();
                    if (!string.IsNullOrEmpty(matchCode))
                    {
                        Console.WriteLine($"✅ User {userId} explicitly joining match {matchCode}");
                        JoinMatchRoom(userId, matchCode);
                        await Send(userId, new { Type = "JOINED_MATCH", Data = new { matchCode } });

                        // 🔥 GỬI CÂU HỎI NGAY SAU KHI CLIENT JOIN
                        await SendQuestionsToMatch(matchCode);
                    }
                    break;

                case "SUBMIT_ANSWER":
                    await HandleSubmitAnswerFromClient(userId, data.Data);
                    break;
            }
        }

        private async Task HandleSubmitAnswerFromClient(int userId, JsonElement? data)
        {
            if (!data.HasValue) return;

            var matchCode = data.Value.GetProperty("matchCode").GetString();
            if (string.IsNullOrEmpty(matchCode)) return;

            var answer = new MatchAnswerModel
            {
                CauHoiID = data.Value.GetProperty("questionId").GetInt32(),
                DapAnDaChon = data.Value.GetProperty("selectedAnswer").GetString()
            };

            await HandleSubmitAnswerAsync(userId, matchCode, answer);
        }

        /* =========================================================
           PRIVATE ROOM LOGIC
        ========================================================= */
        public async Task HandleCreateRoomAsync(int userId)
        {
            string roomCode = Guid.NewGuid().ToString("N")[..6].ToUpper();
            _waitingRooms[roomCode] = new List<int> { userId };

            await Send(userId, new
            {
                Type = "ROOM_CREATED",
                Data = new { roomCode }
            });
        }

        public async Task HandleJoinPrivateRoomAsync(int userId, string roomCode)
        {
            if (!_waitingRooms.TryGetValue(roomCode, out var room))
            {
                await Send(userId, new { Type = "ERROR", Data = new { message = "Phòng không tồn tại" } });
                return;
            }

            lock (room)
            {
                if (room.Count >= 2)
                {
                    Send(userId, new { Type = "ERROR", Data = new { message = "Phòng đã đủ người" } });
                    return;
                }
                room.Add(userId);
            }

            if (room.Count == 2)
            {
                _waitingRooms.TryRemove(roomCode, out _);
                await StartPrivateMatch(room[0], room[1]);
            }
        }

        private async Task StartPrivateMatch(int p1, int p2)
        {
            using var scope = _serviceProvider.CreateScope();
            var matchService = scope.ServiceProvider.GetRequiredService<IOnlineMatchService>();

            string matchCode = await matchService.CreateMatchWithBothPlayersAsync(p1, p2);
            if (string.IsNullOrEmpty(matchCode)) return;

            Console.WriteLine($"🎮 Starting match {matchCode} between {p1} and {p2}");

            // Join both players to match room NGAY LẬP TỨC
            JoinMatchRoom(p1, matchCode);
            JoinMatchRoom(p2, matchCode);

            // Notify both players
            await Send(p1, new { Type = "MATCH_FOUND", Data = new { matchCode, opponentId = p2, yourRole = "Player1" } });
            await Send(p2, new { Type = "MATCH_FOUND", Data = new { matchCode, opponentId = p1, yourRole = "Player2" } });

            Console.WriteLine($"✅ Both players notified, waiting for them to join match page...");
        }

        // 🔥 PHƯƠNG THỨC MỚI - GỬI CÂU HỎI CHỈ KHI CẢ 2 PLAYERS ĐÃ JOIN
        private async Task SendQuestionsToMatch(string matchCode)
        {
            // Kiểm tra xem đã gửi câu hỏi chưa
            if (!_matchRooms.TryGetValue(matchCode, out var players))
            {
                Console.WriteLine($"⚠️ Match room {matchCode} not found when trying to send questions");
                return;
            }

            // Chỉ gửi khi CẢ 2 players đã join
            if (players.Count < 2)
            {
                Console.WriteLine($"⏳ Waiting for both players to join {matchCode}. Current: {players.Count}/2");
                return;
            }

            Console.WriteLine($"📝 Both players ready in {matchCode}, fetching questions...");

            using var scope = _serviceProvider.CreateScope();
            var matchService = scope.ServiceProvider.GetRequiredService<IOnlineMatchService>();

            var questions = await matchService.GetQuestionsByMatchCodeAsync(matchCode);

            if (questions == null || !questions.Any())
            {
                Console.WriteLine($"❌ ERROR: No questions found for match {matchCode}!");
                await Broadcast(matchCode, new
                {
                    Type = "ERROR",
                    Data = new { message = "Không tìm thấy câu hỏi cho trận đấu này!" }
                });
                return;
            }

            Console.WriteLine($"✅ Found {questions.Count()} questions for match {matchCode}");

            await Broadcast(matchCode, new
            {
                Type = "QUESTIONS",
                Data = questions
            });

            Console.WriteLine($"📤 Questions broadcasted to {matchCode}");
        }

        /* =========================================================
           RANDOM MATCH
        ========================================================= */
        public async Task HandleFindRandomMatchAsync(int userId)
        {
            int opponent = 0;

            lock (_queueLock)
            {
                if (!_randomQueue.Contains(userId))
                    _randomQueue.Add(userId);

                opponent = _randomQueue.FirstOrDefault(id => id != userId);
                if (opponent != 0)
                {
                    _randomQueue.Remove(userId);
                    _randomQueue.Remove(opponent);
                }
            }

            if (opponent != 0)
                await StartPrivateMatch(userId, opponent);
            else
                await Send(userId, new { Type = "WAITING_FOR_MATCH" });
        }

        /* =========================================================
           GAME LOGIC
        ========================================================= */
        public async Task HandleSubmitAnswerAsync(int userId, string matchCode, MatchAnswerModel answer)
        {
            using var scope = _serviceProvider.CreateScope();
            var matchService = scope.ServiceProvider.GetRequiredService<IOnlineMatchService>();

            bool correct = await matchService
                .SubmitAnswerByMatchCodeAsync(matchCode, userId, answer);

            await Broadcast(matchCode, new
            {
                Type = "SCORE_UPDATE",
                Data = new
                {
                    userId,
                    questionId = answer.CauHoiID,
                    correct,
                    selectedAnswer = answer.DapAnDaChon
                }
            });

            await CheckMatchCompletion(matchCode);
        }

        private async Task CheckMatchCompletion(string matchCode)
        {
            using var scope = _serviceProvider.CreateScope();
            var matchService = scope.ServiceProvider.GetRequiredService<IOnlineMatchService>();

            var result = await matchService.EndMatchByCodeAsync(matchCode);
            if (result.KetQua == "Wait") return;

            await Broadcast(matchCode, new
            {
                Type = "GAME_END",
                Data = result
            });
        }

        /* =========================================================
           SOCKET UTILITIES
        ========================================================= */
        private void JoinMatchRoom(int userId, string matchCode)
        {
            var players = _matchRooms.GetOrAdd(matchCode, _ => new List<int>());
            lock (players)
            {
                if (!players.Contains(userId))
                {
                    players.Add(userId);
                    Console.WriteLine($"✅ User {userId} joined match room {matchCode}. Total players: {players.Count}");
                }
            }
        }

        public void Register(int userId, WebSocket socket)
        {
            _userSockets[userId] = socket;
            Console.WriteLine($"🔌 User {userId} registered. Total online: {_userSockets.Count}");
        }

        public void Unregister(int userId)
        {
            _userSockets.TryRemove(userId, out _);
            Console.WriteLine($"🔌 User {userId} disconnected. Total online: {_userSockets.Count}");
        }

        public async Task Send(int userId, object message)
        {
            if (!_userSockets.TryGetValue(userId, out var socket))
            {
                Console.WriteLine($"⚠️ Cannot send to user {userId} - socket not found");
                return;
            }

            if (socket.State != WebSocketState.Open)
            {
                Console.WriteLine($"⚠️ Cannot send to user {userId} - socket not open ({socket.State})");
                return;
            }

            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"📤 Sent to user {userId}: {json.Substring(0, Math.Min(100, json.Length))}...");
        }

        public async Task Broadcast(string matchCode, object message)
        {
            if (!_matchRooms.TryGetValue(matchCode, out var players))
            {
                Console.WriteLine($"❌ Match room {matchCode} not found! Cannot broadcast.");
                return;
            }

            Console.WriteLine($"📢 Broadcasting to match {matchCode} ({players.Count} players)");

            foreach (var uid in players)
            {
                Console.WriteLine($"  → Sending to user {uid}");
                await Send(uid, message);
            }
        }

        public int GetOnlineCount() => _userSockets.Count;
    }

    public class ClientMessage
    {
        public string Type { get; set; }
        public JsonElement? Data { get; set; }
    }
}