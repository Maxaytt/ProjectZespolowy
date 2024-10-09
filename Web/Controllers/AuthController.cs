using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Domain.ViewModels;


namespace Web.Controllers;

public class AuthController(SignInManager<User> signIn, UserManager<User> userManager) : Controller
{
    private readonly SignInManager<User> signIn = signIn;
    private readonly UserManager<User> userManager = userManager;

    [HttpGet]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await signIn.UserManager.FindByEmailAsync(email);
        if (user is null) return NotFound($"User {email} not found");

        var result = await signIn.PasswordSignInAsync(user, password, false, false);

        if (!result.Succeeded)
        {
            throw new Exception("Login fail");
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        var model = new RegisterViewModel();
        return View("Register", model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Login");
        }

        var user = await signIn.UserManager.FindByEmailAsync(model.Email);
        if (user is not null) return Conflict($"User {model.Email} already exists");

        user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Email,
            Email = model.Email
        };
        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            throw new Exception("Authentication failed");
        }

        return RedirectToAction("Login", "Auth");
    }
    
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction(nameof(Login), "Auth");
    }
}