namespace NorthboundSessions.Data;

public class RealtimeProbe
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }
}
