using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;


namespace Web.Controllers;

/// <summary>
/// Controller responsible for handling user authentication processes.
/// </summary>
public class AuthController(SignInManager<User> signIn, UserManager<User> userManager) : Controller
{
    private readonly SignInManager<User> signIn = signIn;
    private readonly UserManager<User> userManager = userManager;

    /// <summary>
    /// Displays the login page.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View("Login");
    }

    // <summary>
    /// Handles user login attempts.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>Redirects to the home page if successful or returns an error.</returns>
    /// <exception cref="Exception">Thrown when login fails.</exception>
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

    /// <summary>
    /// Displays the registration page.
    /// </summary>
    /// <returns>The registration view with an empty model.</returns>
    [HttpGet]
    public IActionResult Register()
    {
        var model = new RegisterViewModel();
        return View("Register", model);
    }

    /// <summary>
    /// Handles user registration.
    /// </summary>
    /// <param name="model">The registration model containing user data.</param>
    /// <returns>Redirects to the login page if successful or reloads the registration page.</returns>
    /// <exception cref="Exception">Thrown when user registration fails.</exception>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Register");
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

    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    /// <returns>Redirects to the login page</returns>
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction(nameof(AuthController.Login), "Auth");
    }
}