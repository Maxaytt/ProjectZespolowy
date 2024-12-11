namespace Domain.Models;

public class TestResult
{
    public Guid Id { get; set; }         
    public Guid UserId { get; set; }      
    public Guid FilmId { get; set; }
    public Film Film { get; set; } 
    public DateTime Timestamp { get; set; }  
    public List<Answer> Answers { get; set; } = [];
    public int CorrectAnswers => Answers.Count(answer => answer.IsTrue);
}