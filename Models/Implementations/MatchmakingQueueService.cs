using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using QUIZ_GAME_WEB.Hubs;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Collections.Concurrent;

public class MatchmakingQueueService : IMatchmakingQueueService
{
    private static readonly List<int> _queue = new();
    private static readonly object _locker = new();

    private static readonly ConcurrentDictionary<string, int> _rooms = new();

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<MatchmakingHub> _hub;

    public MatchmakingQueueService(IServiceScopeFactory scopeFactory, IHubContext<MatchmakingHub> hub)
    {
        _scopeFactory = scopeFactory;
        _hub = hub;
    }

    private string GenCode()
    {
        const string c = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789";
        var r = new Random();
        return new string(Enumerable.Repeat(c, 4)
            .Select(x => x[r.Next(x.Length)]).ToArray());
    }

    // =====================================
    // ADD QUEUE
    // =====================================
    public Task AddToQueueAsync(int id)
    {
        lock (_locker)
        {
            if (!_queue.Contains(id))
                _queue.Add(id);
        }

        return Task.CompletedTask;
    }

    public Task RemoveFromQueueAsync(int id)
    {
        lock (_locker)
        {
            _queue.Remove(id);
        }

        return Task.CompletedTask;
    }

    // =====================================
    // CHECK MATCH
    // =====================================
    public async Task CheckRandomMatchAsync()
    {
        int p1 = 0, p2 = 0;

        lock (_locker)
        {
            if (_queue.Count >= 2)
            {
                p1 = _queue[0];
                p2 = _queue[1];

                _queue.RemoveAt(0);
                _queue.RemoveAt(0);
            }
        }

        if (p1 == 0 || p2 == 0)
            return;

        using var scope = _scopeFactory.CreateScope();
        var matchService = scope.ServiceProvider.GetRequiredService<IOnlineMatchService>();

        string matchCode = await matchService.CreateMatchAsync(p1, p2);
        var questions = await matchService.GetQuestionsByMatchCodeAsync(matchCode);

        await _hub.Clients.User(p1.ToString()).SendAsync("MatchFound", new
        {
            MatchCode = matchCode,
            OpponentId = p2,
            Questions = questions
        });

        await _hub.Clients.User(p2.ToString()).SendAsync("MatchFound", new
        {
            MatchCode = matchCode,
            OpponentId = p1,
            Questions = questions
        });
    }

    // =====================================
    // PRIVATE ROOM
    // =====================================
    public Task<string> CreatePrivateRoomAsync(int creatorId)
    {
        string code = GenCode();
        _rooms.TryAdd(code, creatorId);
        return Task.FromResult(code);
    }

    public async Task<(string matchCode, int creatorId)> JoinPrivateRoomAsync(string roomCode, int joinerId)
    {
        if (!_rooms.TryRemove(roomCode, out int creatorId))
            throw new Exception("Phòng không tồn tại.");

        using var scope = _scopeFactory.CreateScope();
        var matchService = scope.ServiceProvider.GetRequiredService<IOnlineMatchService>();

        string matchCode = await matchService.CreateMatchAsync(creatorId, joinerId);
        var questions = await matchService.GetQuestionsByMatchCodeAsync(matchCode);

        await _hub.Clients.User(creatorId.ToString()).SendAsync("MatchFound", new
        {
            MatchCode = matchCode,
            OpponentId = joinerId,
            Questions = questions
        });

        await _hub.Clients.User(joinerId.ToString()).SendAsync("MatchFound", new
        {
            MatchCode = matchCode,
            OpponentId = creatorId,
            Questions = questions
        });

        return (matchCode, creatorId);
    }
}
