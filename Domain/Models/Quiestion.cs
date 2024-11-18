namespace Domain.Models;

public class Question
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public Guid FilmId { get; set; }
    public Film Film { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = null!;
}