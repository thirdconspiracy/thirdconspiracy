namespace poker.api.Models;

public class Player
{
    public required string ConnectionId { get; set; }
    public required string Name { get; set; }
    public required string GroupName { get; set; }
    public Estimate Estimate { get; set; }
    public bool IsPlaying { get; set; }
}