using poker.api.Models;
using poker.api.Repos;

namespace poker.api.Services;

public class PokerService(IPokerRepository pokerRepository) : IPokerService
{
    public string GroupName(string tableId) => $"table-{tableId}";

    public async Task<PokerTable> JoinTable(string tableId, string connectionId, string playerName, bool isPlaying = true)
    {
        var player = new Player
        {
            ConnectionId = connectionId,
            Name = playerName,
            GroupName = GroupName(tableId),
            Estimate = Estimate.None,
            IsPlaying = isPlaying
        };
        var table = await pokerRepository.JoinTable(tableId, player);
        return table;
    }

    public async Task<PokerTable?> FindTable(string connectionId)
    {
        return await pokerRepository.GetTable(connectionId);
    }

    public async Task<Player?> LeaveTable(string tableId, string connectionId)
    {
        var player = await pokerRepository.LeaveTable(tableId, connectionId);
        return player;
    }

    public async Task CleanTable(string tableId)
    {
        await pokerRepository.CleanTable(tableId);
    }

    public async Task<bool> Deal(string tableId)
    {
        await pokerRepository.Deal(tableId);
        return true;
    }

    public async Task<bool> SetShow(string tableId, bool show)
    {
        return await pokerRepository.SetShow(tableId, show);
    }

    public async Task<Player?> SetEstimate(string tableId, string connectionId, Estimate estimate)
    {
        return await pokerRepository.SetEstimate(tableId, connectionId, estimate);
    }

    public async Task<Player?> SetIsPlaying(string tableId, string connectionId, bool isPlaying)
    {
        return await pokerRepository.SetIsPlaying(tableId, connectionId, isPlaying);
    }
}