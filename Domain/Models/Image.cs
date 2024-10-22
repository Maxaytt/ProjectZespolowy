namespace Domain.Models;

public class Image
{
    public Guid Id { get; set; }
    public byte[] Content { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string Caption { get; set; } = null!;
    public Film Film { get; set; } = null!;
    public Guid FilmId { get; set; }
}