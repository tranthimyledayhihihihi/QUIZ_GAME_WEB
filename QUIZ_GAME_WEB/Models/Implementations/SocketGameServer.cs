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
        // MATCH COMPLETION TRACKING
        // ===============================
        private readonly ConcurrentDictionary<string, bool> _matchCompleted = new();
        private readonly ConcurrentDictionary<string, object> _matchLocks = new();

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

                case "JOIN_MATCH":
                    var matchCode = data.Data?.GetProperty("matchCode").GetString();
                    if (!string.IsNullOrEmpty(matchCode))
                    {
                        Console.WriteLine($"✅ User {userId} explicitly joining match {matchCode}");
                        JoinMatchRoom(userId, matchCode);
                        await Send(userId, new { Type = "JOINED_MATCH", Data = new { matchCode } });

                        // Gửi câu hỏi ngay sau khi client join
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

            _ = Task.Delay(TimeSpan.FromMinutes(1)).ContinueWith(async _ =>
            {
                if (!_waitingRooms.TryGetValue(roomCode, out var room))
                    return;

                lock (room)
                {
                    // ❌ Không expire nếu phòng đã đủ người
                    if (room.Count >= 2)
                        return;
                }

                if (_waitingRooms.TryRemove(roomCode, out var removedRoom))
                {
                    Console.WriteLine($"⏰ Room {roomCode} expired after 5 minutes");

                    foreach (var playerId in removedRoom)
                    {
                        await Send(playerId, new
                        {
                            Type = "ROOM_EXPIRED",
                            Data = new { message = "Phòng đã hết hạn (5 phút không có người vào)" }
                        });
                    }
                }
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
                _waitingRooms.TryRemove(roomCode, out List<int> removedRoom);
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

            // Join both players to match room ngay lập tức
            JoinMatchRoom(p1, matchCode);
            JoinMatchRoom(p2, matchCode);

            // Notify both players
            await Send(p1, new { Type = "MATCH_FOUND", Data = new { matchCode, opponentId = p2, yourRole = "Player1" } });
            await Send(p2, new { Type = "MATCH_FOUND", Data = new { matchCode, opponentId = p1, yourRole = "Player2" } });

            Console.WriteLine($"✅ Both players notified, waiting for them to join match page...");
        }

        /* =========================================================
           SEND QUESTIONS TO MATCH
        ========================================================= */
        private async Task SendQuestionsToMatch(string matchCode)
        {
            if (!_matchRooms.TryGetValue(matchCode, out var players))
            {
                Console.WriteLine($"⚠️ Match room {matchCode} not found when trying to send questions");
                return;
            }

            // Chỉ gửi khi cả 2 players đã join
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
            // 🔥 KIỂM TRA NGAY TỪ ĐẦU ĐỂ TRÁNH GỌI SERVICE NHIỀU LẦN
            var lockObj = _matchLocks.GetOrAdd(matchCode, _ => new object());

            bool shouldBroadcast = false;
            lock (lockObj)
            {
                if (_matchCompleted.ContainsKey(matchCode))
                {
                    Console.WriteLine($"⚠️ Match {matchCode} already completed, skipping");
                    return;
                }

                // Đánh dấu đang xử lý để tránh duplicate
                _matchCompleted[matchCode] = false; // false = đang xử lý
                shouldBroadcast = true;
            }

            if (!shouldBroadcast) return;

            using var scope = _serviceProvider.CreateScope();
            var matchService = scope.ServiceProvider.GetRequiredService<IOnlineMatchService>();

            var result = await matchService.EndMatchByCodeAsync(matchCode);

            if (result.KetQua == "Wait")
            {
                // Nếu chưa xong thì xóa flag
                lock (lockObj)
                {
                    _matchCompleted.TryRemove(matchCode, out _);
                }
                return;
            }

            // ✅ XÁC NHẬN TRẬN ĐẤU ĐÃ KẾT THÚC
            lock (lockObj)
            {
                _matchCompleted[matchCode] = true; // true = đã hoàn thành
            }

            Console.WriteLine($"🏁 Match {matchCode} completed! Result: {result.KetQua}");

            // 🔥 ĐẢM BẢO CẢ 2 NGƯỜI CHƠI ĐỀU NHẬN ĐƯỢC KẾT QUẢ
            try
            {
                if (!_matchRooms.TryGetValue(matchCode, out var players))
                {
                    Console.WriteLine($"❌ Match room {matchCode} not found!");
                    return;
                }

                Console.WriteLine($"📢 Broadcasting GAME_END to {players.Count} players in {matchCode}");

                // 🔥 GỬI RIÊNG TỪNG NGƯỜI VÀ ĐỢI XÁC NHẬN
                var sendTasks = new List<Task>();
                foreach (var playerId in players.ToList())
                {
                    sendTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await Send(playerId, new
                            {
                                Type = "GAME_END",
                                Data = result
                            });

                            // Gửi lại lần 2 sau 500ms để đảm bảo
                            await Task.Delay(500);
                            await Send(playerId, new
                            {
                                Type = "GAME_END",
                                Data = result
                            });

                            Console.WriteLine($"  ✅ Sent GAME_END to player {playerId} (x2)");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"  ❌ Failed to send to player {playerId}: {ex.Message}");
                        }
                    }));
                }

                await Task.WhenAll(sendTasks);
                Console.WriteLine($"✅ All GAME_END messages sent for match {matchCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in CheckMatchCompletion for {matchCode}: {ex.Message}");
            }

            // Cleanup sau 15 giây (tăng thời gian để user có thể xem kết quả)
            _ = Task.Delay(150000).ContinueWith(t =>
            {
                _matchRooms.TryRemove(matchCode, out _);
                _matchCompleted.TryRemove(matchCode, out _);
                _matchLocks.TryRemove(matchCode, out _);
                Console.WriteLine($"🧹 Cleaned up match {matchCode}");
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

            // 🔥 XÓA USER KHỎI TẤT CẢ MATCH ROOMS
            foreach (var kvp in _matchRooms)
            {
                var players = kvp.Value;
                lock (players)
                {
                    if (players.Remove(userId))
                    {
                        Console.WriteLine($"🔌 Removed user {userId} from match room {kvp.Key}");
                    }
                }
            }

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

            try
            {
                var json = JsonSerializer.Serialize(message);
                var buffer = Encoding.UTF8.GetBytes(json);

                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine($"📤 Sent to user {userId}: {json.Substring(0, Math.Min(100, json.Length))}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending to user {userId}: {ex.Message}");
            }
        }

        public async Task Broadcast(string matchCode, object message)
        {
            if (!_matchRooms.TryGetValue(matchCode, out var players))
            {
                Console.WriteLine($"❌ Match room {matchCode} not found! Cannot broadcast.");
                return;
            }

            Console.WriteLine($"📢 Broadcasting to match {matchCode} ({players.Count} players)");

            // 🔥 SỬ DỤNG ToList() ĐỂ TRÁNH COLLECTION MODIFIED EXCEPTION
            var playersList = players.ToList();
            var tasks = new List<Task>();

            foreach (var uid in playersList)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        Console.WriteLine($"  → Sending to user {uid}");
                        await Send(uid, message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Failed to send to user {uid}: {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"✅ Broadcast completed for match {matchCode}");
        }

        public int GetOnlineCount() => _userSockets.Count;
    }

    public class ClientMessage
    {
        public string Type { get; set; }
        public JsonElement? Data { get; set; }
    }
}