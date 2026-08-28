namespace NorthboundSessions.Data;

public class Attendance
{
    public int Id { get; set; }
    public required string StudentId { get; set; }
    public DateTimeOffset CheckedInAt { get; set; } = DateTimeOffset.UtcNow;
    public int LiveSessionId {get; set;}
    public LiveSession? LiveSession {get; set;}
    
}

