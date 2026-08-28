using System.Collections.Generic;

namespace NorthboundSessions.Data;

public class Lesson
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string OutlineContent { get; set; }
    public string? MarketSymbol { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public bool IsLiveDay { get; set; }
    public string? SlideFilePath { get; set; }
    public string? HandoutFilePath { get; set; }
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}



