namespace Domain.ViewModel
{
    public class AddAnswerVm
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string Text { get; set; } = null!;
        public bool IsTrue { get; set; }

        public List<AnswerVm> Answers { get; set; } = new();
    }

    public class AnswerVm
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsTrue { get; set; }
    }
}
