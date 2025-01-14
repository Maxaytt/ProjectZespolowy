using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Domain.ViewModels;
using Mapster;

namespace Web.Controllers;

/// <summary>
/// Controller for handling the home page of the application, including fetching and displaying films.
/// </summary>
public class HomeController : Controller
{
    private AppDbContext dbContext { get; set; }
    
    /// <summary>
    /// Constructs the controller with dependency injection for the database context.
    /// </summary>
    /// <param name="_dbContext">The database context for accessing application data.</param>
    public HomeController(AppDbContext _dbContext)
    {
        dbContext = _dbContext;
    }

    /// <summary>
    /// Displays the main index page of the application, which includes a list of films.
    /// </summary>
    /// <returns>A view containing a list of films, each with its associated image.</returns>
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var films = await dbContext.Films
            .Include(p => p.Image)
            .ToListAsync();

        var vmList = films.Adapt<List<FilmVm>>();

        return View(vmList);
    }

    /// <summary>
    /// Displays the Privacy page of the application.
    /// </summary>
    /// <returns>The Privacy view.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays an error page with detailed information if an error occurs in the application.
    /// This page is typically used when a request fails.
    /// </summary>
    /// <returns>The Error view, displaying the request ID and other relevant error information.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}