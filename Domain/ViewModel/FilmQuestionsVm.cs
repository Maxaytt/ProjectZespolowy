using Domain.Models;

namespace Domain.ViewModel
{
    public class FilmQuestionsVm
    {
        public Guid FilmId { get; set; }
        public string FilmName { get; set; }
        public List<Question> Questions { get; set; }
    }
}
