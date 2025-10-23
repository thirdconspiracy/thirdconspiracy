using System.Collections.Concurrent;
using poker.api.Models;

namespace poker.api.Repos;

public class PokerRepository : IPokerRepository
{
    private readonly ConcurrentDictionary<string, PokerTable> _activeTables = new();

    public async Task<PokerTable> JoinTable(string tableId, Player player)
    {
        var table = _activeTables.GetOrAdd(tableId, new PokerTable {Id = tableId});
        table.Players[player.ConnectionId] = player;
        return await Task.FromResult(table);
    }

    public Task<PokerTable?> GetTable(string connectionId)
    {
        var table = _activeTables.Values
            .FirstOrDefault(table => table.Players.ContainsKey(connectionId));
        return Task.FromResult(table);
    }

    public async Task<Player?> LeaveTable(string tableId, string connectionId)
    {
        if (!_activeTables.TryGetValue(tableId, out var table))
            return null;

        if (!table.Players.TryRemove(connectionId, out var player))
            return null;

        await CleanTable(tableId);
        
        return player;
    }

    public Task CleanTable(string tableId)
    {
        if (_activeTables.TryGetValue(tableId, out var table) && table.Players.IsEmpty)
            _activeTables.TryRemove(tableId, out _);

        return Task.CompletedTask;
    }

    public Task Deal(string tableId)
    {
        if (!_activeTables.TryGetValue(tableId, out var table))
            return Task.CompletedTask;

        foreach (var player in table.Players.Values)
            player.Estimate = Estimate.None;

        table.Show = false;
        return Task.CompletedTask;
    }

    public Task<bool> SetShow(string tableId, bool show)
    {
        if (!_activeTables.TryGetValue(tableId, out var table))
            return Task.FromResult(false);

        table.Show = show;
        return Task.FromResult(show);
    }

    public Task<Player?> SetEstimate(string tableId, string connectionId, Estimate estimate)
    {
        if (!_activeTables.TryGetValue(tableId, out var table))
            return Task.FromResult<Player?>(null);

        if (!table.Players.TryGetValue(connectionId, out var player))
            return Task.FromResult<Player?>(null);

        player.Estimate = estimate;
        return Task.FromResult<Player?>(player);
    }

    public Task<Player?> SetIsPlaying(string tableId, string connectionId, bool isPlaying)
    {
        if (!_activeTables.TryGetValue(tableId, out var table))
            return Task.FromResult<Player?>(null);

        if (!table.Players.TryGetValue(connectionId, out var player))
            return Task.FromResult<Player?>(null);

        player.IsPlaying = isPlaying;
        return Task.FromResult<Player?>(player);

    }
}