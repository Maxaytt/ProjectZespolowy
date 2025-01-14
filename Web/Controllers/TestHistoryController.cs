using System.Security.Claims;
using Domain.ViewModels;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Web.Controllers;

/// <summary>
/// Controller responsible for fetching and displaying the test history of the logged-in user.
/// </summary>
[Route("[controller]")]
public class TestHistoryController(AppDbContext context) : Controller
{
    /// <summary>
    /// Fetches and displays the test history of the currently logged-in user.
    /// Retrieves test results from the database based on the user's ID stored in the claim and returns a list of test results.
    /// </summary>
    /// <returns>
    /// A view displaying the test history for the current user. 
    /// If the user is not found, the user ID is invalid, or there are no test results, an appropriate error or empty view will be shown.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetTestHistory()
    {
        var userIdClaim = HttpContext.User.Claims
            .ToList()
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim?.Value, out var userId))
        {
            return BadRequest("Invalid user ID format.");
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return NotFound($"User with ID '{userId}' not found.");
        }

        var testHistory = await context.TestResults
            .Where(result => result.UserId == user.Id)
            .Select(result => new GetTestHistoryVm
            {
                Filmname = result.Film.Name,
                Timestamp = result.Timestamp,
                CorrectAnswers = result.CorrectAnswers,
                AnswersCount = result.Answers.Count
            })
            .ToListAsync();

        return View(testHistory);
    }
}