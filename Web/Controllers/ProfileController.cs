using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using System.Security.Claims;

namespace Web.Controllers;

/// <summary>
/// Controller responsible for handling user profile-related actions.
/// </summary>
public class ProfileController : Controller
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileController"/> with the provided database context.
    /// </summary>
    /// <param name="context">The database context used to interact with the application data.</param>
    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays the profile page for the currently logged-in user.
    /// Fetches the user details from the database based on the user's ID stored in the claim.
    /// </summary>
    /// <returns>
    /// A view displaying the user's profile information. 
    /// If the user is not found or the user ID is invalid, returns an error response.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdClaim = HttpContext.User.Claims
            .ToList()
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim?.Value, out var userId))
        {
            return BadRequest("Invalid user ID format.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return NotFound($"User with ID '{userId}' not found.");
        }

        return View(user);
    }

    /// <summary>
    /// Displays the edit page where the user can update their profile information.
    /// The action fetches the current user's profile details from the database.
    /// </summary>
    /// <returns>
    /// A view displaying the edit form pre-filled with the user's current profile details.
    /// If the user is not found or the user ID is invalid, returns an error response.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userIdClaim = HttpContext.User.Claims
            .ToList()
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim?.Value, out var userId))
        {
            return BadRequest("Invalid user ID format.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return NotFound($"User with ID '{userId}' not found.");
        }

        return View(user);
    }

    /// <summary>
    /// Handles the POST request to update the user's profile information.
    /// Updates the user's details (first name, last name, email) in the database.
    /// </summary>
    /// <param name="model">The updated user model with new profile information.</param>
    /// <returns>
    /// Redirects to the user's profile page after successful update. 
    /// If the model is invalid or the user cannot be found, it returns the user to the edit form.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Edit(User model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user is null)
            {
                return NotFound($"User with ID '{model.Id}' not found.");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }
}