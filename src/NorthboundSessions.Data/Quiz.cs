namespace NorthboundSessions.Data;

public class Quiz
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public required string Title { get; set; }
    public Lesson? Lesson { get; set; }
    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
}
