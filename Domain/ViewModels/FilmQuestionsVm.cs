using Domain.Models;

namespace Domain.ViewModels;

public class FilmQuestionsVm
{
    public Guid FilmId { get; set; }
    public string FilmName { get; set; }
    public List<Question> Questions { get; set; }
}
