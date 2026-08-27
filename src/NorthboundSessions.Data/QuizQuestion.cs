namespace NorthboundSessions.Data;

public class QuizQuestion
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public required string QuestionText { get; set; }
    public int DisplayOrder { get; set; }
    public Quiz? Quiz { get; set; }
    public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
}
