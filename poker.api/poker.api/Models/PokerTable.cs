using System.Collections.Concurrent;

namespace poker.api.Models;

public class PokerTable
{
    public required string Id { get; init; }
    public bool Show { get; set; }
    public ConcurrentDictionary<string, Player> Players { get; } = [];
}