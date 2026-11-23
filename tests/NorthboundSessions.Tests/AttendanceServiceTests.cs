using NorthboundSessions.Data;
using NorthboundSessions.Web.Services;

namespace NorthboundSessions.Tests;

public class AttendanceServiceTests
{
    private static readonly DateTimeOffset Start = new(2026, 11, 23, 18, 0, 0, TimeSpan.Zero);

    private static LiveSession Session => new()
    {
        Topic = "Test session",
        MeetLink = "https://example.com/meet",
        ScheduledAt = Start
    };

    [Fact]
    public void IsCheckInOpen_AcceptsExactWindowBoundaries()
    {
        Assert.True(AttendanceService.IsCheckInOpen(Session, Start - TimeSpan.FromMinutes(30)));
        Assert.True(AttendanceService.IsCheckInOpen(Session, Start));
        Assert.True(AttendanceService.IsCheckInOpen(Session, Start + TimeSpan.FromHours(2)));
    }

    [Fact]
    public void IsCheckInOpen_RejectsTimesOutsideWindow()
    {
        Assert.False(AttendanceService.IsCheckInOpen(Session, Start - TimeSpan.FromMinutes(31)));
        Assert.False(AttendanceService.IsCheckInOpen(Session, Start + TimeSpan.FromHours(2) + TimeSpan.FromSeconds(1)));
    }
}
