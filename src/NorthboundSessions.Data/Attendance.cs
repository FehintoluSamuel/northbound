namespace NorthboundSessions.Data;

public class Attendance
{
    public int Id { get; set; }
    public required string StudentId { get; set; }
    public DateOnly SessionDate { get; set; }
    public DateTimeOffset CheckedInAt { get; set; } = DateTimeOffset.UtcNow;
}
