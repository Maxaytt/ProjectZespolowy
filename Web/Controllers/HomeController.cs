using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Domain.ViewModel;
using Mapster;

namespace Web.Controllers;

public class HomeController : Controller
{
    private AppDbContext dbContext { get; set; }
    public HomeController(AppDbContext _dbContext)
    {
        dbContext = _dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var films = await dbContext.Films
            .Include(p => p.Image)
            .ToListAsync();

        var vmList = films.Adapt<List<FilmVm>>();

        return View(vmList);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}