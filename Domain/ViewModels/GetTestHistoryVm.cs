namespace Domain.ViewModels;

public class GetTestHistoryVm
{
    public string Filmname { get; set; } 
    public DateTime Timestamp { get; set; }  
    public int CorrectAnswers { get; set; }
    public int AnswersCount { get; set; }
}