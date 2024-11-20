namespace Domain.ViewModel
{
    public class AddAnswerVm
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string Text { get; set; } = null!;
        public bool IsTrue { get; set; }

        public List<(Guid Id, string Text, bool IsTrue)> Answers { get; set; } = new();
    }

}
