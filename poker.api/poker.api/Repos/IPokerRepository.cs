using poker.api.Models;

namespace poker.api.Repos;

public interface IPokerRepository
{
    Task<PokerTable> JoinTable(string tableId, Player player);
    Task<PokerTable?> GetTable(string connectionId);
    Task<Player?> LeaveTable(string tableId, string connectionId);
    Task CleanTable(string tableId);
    Task Deal(string tableId);
    Task<bool> SetShow(string tableId, bool show);
    Task<Player?> SetEstimate(string tableId, string connectionId, Estimate estimate);
    Task<Player?> SetIsPlaying(string tableId, string connectionId, bool isPlaying);
}