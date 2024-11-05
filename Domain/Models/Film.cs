namespace Domain.Models;
public class Film
{
    public Guid Id { get; set; }
    public byte[] Content { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<Question> Questions { get; set; } = [];
    public Image Image { get; set; } = null!;
    public Guid ImageId { get; set; }

}

