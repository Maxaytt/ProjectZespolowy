using Microsoft.AspNetCore.Http;

namespace Domain.ViewModel;
public class CreateEditFilmVm
{
    public Guid Id { get; set; } = new Guid();
    public IFormFile VideoFile { get; set; } = null!;
    public IFormFile ImageFile { get; set; } = null!;
    public string Name { get; set; } = null!;
}