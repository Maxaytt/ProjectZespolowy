using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModels;

public class RegisterViewModel
{
    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
