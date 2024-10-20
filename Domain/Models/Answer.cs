namespace Domain.Models;

public class Answer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public string Text { get; set; } = null!;
    public bool IsTrue { get; set; }
}