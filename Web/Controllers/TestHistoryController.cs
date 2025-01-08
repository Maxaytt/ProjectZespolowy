using System.Security.Claims;
using Domain.ViewModels;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Web.Controllers;

[Route("[controller]")]
public class TestHistoryController(AppDbContext context) : Controller
{
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