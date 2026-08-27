namespace NorthboundSessions.Data;

public class QuizOption
{
    public int Id { get; set; }
    public int QuizQuestionId { get; set; }
    public required string OptionText { get; set; }
    public bool IsCorrect { get; set; }
    public int DisplayOrder { get; set; }
    public QuizQuestion? QuizQuestion { get; set; }
}
