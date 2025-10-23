using poker.api.Models;

namespace poker.api.Services;

public interface IPokerService
{
    string GroupName(string tableId);
    Task<PokerTable> JoinTable(string tableId, string connectionId, string playerName, bool isPlaying = true);
    Task<PokerTable?> FindTable(string connectionId);
    Task<Player?> LeaveTable(string tableId, string connectionId);
    Task CleanTable(string tableId);
    Task<bool> Deal(string tableId);
    Task<bool> SetShow(string tableId, bool show);
    Task<Player?> SetEstimate(string tableId, string connectionId, Estimate estimate);
    Task<Player?> SetIsPlaying(string tableId, string connectionId, bool isPlaying);
}