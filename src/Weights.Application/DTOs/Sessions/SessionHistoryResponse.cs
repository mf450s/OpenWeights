namespace Weights.Application.DTOs.Sessions;

public class SessionHistoryResponse
{
    public List<SessionHistoryItem> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
}

public class SessionHistoryItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalVolume { get; set; }
}
