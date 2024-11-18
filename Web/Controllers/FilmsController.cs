using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;

namespace Web.Controllers;

[Route("[controller]")]
public class FilmsController : Controller
{
    private readonly AppDbContext _dbContext;

    public FilmsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var film = _dbContext.Films.Find(id);
        if (film == null)
            return NotFound();

        return Ok(film);
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Create(CreateEditFilmVm film)
    {
        var imageForDatabse = new Image
        {
            Id = Guid.NewGuid(),
            Caption = film.Name,
            ContentType = film.ImageFile.ContentType,
        };
        var filmForDatabase = new Film
        {
            Id = Guid.NewGuid(),
            Name = film.Name,
            Image = imageForDatabse,
            ContentType = film.VideoFile.ContentType
        };

        if (film.ImageFile is not null)
        {
            using var imageStream = new MemoryStream();
            film.ImageFile.CopyTo(imageStream);
            imageForDatabse.Content = imageStream.ToArray();
        }

        if (film.VideoFile is not null)
        {
            using var filmStream = new MemoryStream();
            film.VideoFile.CopyTo(filmStream);
            filmForDatabase.Content = filmStream.ToArray();
        }

        _dbContext.Images.Add(imageForDatabse);
        _dbContext.Films.Add(filmForDatabase);
        _dbContext.SaveChanges();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var film = await _dbContext.Films
            .Include(film => film.Questions)
            .FirstOrDefaultAsync(film => film.Id == id);
        
        if (film is null) return NotFound($"film with id: {id} not found");

        var viewModel = new CreateEditFilmVm
        {
            Id = id,
            Name = film.Name,
            Questions = film.Questions
        };
        return View(viewModel);
    }


    [HttpPost("EditAndAdd")]
    public IActionResult EditAndAdd(CreateEditFilmVm ViewModel)
    {
        var existingFilm = _dbContext.Films
            .Include(f => f.Image)
            .Include(f => f.Questions)
            .First(f => f.Id == ViewModel.Id);

        if (ViewModel.Name is not null)
        {
            existingFilm.Name = ViewModel.Name;
            existingFilm.Image.Caption = ViewModel.Name;
        }

        if (ViewModel.ImageFile is not null)
        {
            using var item = new MemoryStream();
            ViewModel.ImageFile.CopyTo(item);
            existingFilm.Image.Content = item.ToArray();
        }

        if (ViewModel.VideoFile is not null)
        {
            using var item = new MemoryStream();
            ViewModel.VideoFile.CopyTo(item);
            existingFilm.Content = item.ToArray();
        }

        if (ViewModel.NumberOfQuestions >= 0)
        {
            foreach (var question in existingFilm.Questions)
            {
                question.IsIncludedInTest = false;
            }

            var random = new Random();

            var questionsToInclude = existingFilm.Questions
                .AsEnumerable()
                .OrderBy(q => random.Next())
                .Take(ViewModel.NumberOfQuestions)
                .ToList();

            foreach (var question in questionsToInclude)
            {
                question.IsIncludedInTest = true;
            }
        }

        _dbContext.Films.Update(existingFilm);
        _dbContext.SaveChanges();

        return RedirectToAction("Index", "Home");
    }



    [HttpGet("Delete")]
    public IActionResult Delete(Guid id)
    {
        var film = _dbContext.Films.Find(id);
        if (film is null)
            return NotFound();

        _dbContext.Films.Remove(film);
        _dbContext.SaveChanges();

        return RedirectToAction(nameof(Index), "Home");
    }

    [HttpGet("GetFilmAsResource/{id:guid}")]
    public IActionResult GetFilmAsResource(Guid id)
    {
        var film = _dbContext.Films.Find(id);
        if (film is null)
            return NotFound();
        return File(film.Content, film.ContentType, film.Name);
    }

    [HttpGet("GetImageAsResource/{id:guid}")]
    public IActionResult GetImageAsResource(Guid id)
    {
        var image = _dbContext.Images.Find(id);
        if (image is null)
            return NotFound();
        return File(image.Content, image.ContentType, image.Caption);
    }

    [HttpGet("PlayFilm/{id:guid}")]
    public IActionResult PlayFilm(Guid id)
    {
        var film = _dbContext.Films.Find(id);

        if (film == null)
            return NotFound();

        var viewModel = new PlayFilmVm()
        {
            Id = id,
            Name = film.Name,
            ContentType = film.ContentType,
        };
        return View(viewModel);
    }
    
    [HttpGet("DeleteQuestion")]
    public IActionResult DeleteQuestion(Guid id)
    {
        var question = _dbContext.Questions.Find(id);
        if (question is null)
            return NotFound();

        _dbContext.Questions.Remove(question);
        _dbContext.SaveChanges();

        var refererUrl = Request.Headers.Referer.ToString();
        if (!string.IsNullOrEmpty(refererUrl))
        {
            return Redirect(refererUrl);
        }
        
        return RedirectToAction(nameof(Index), "Home");
    }

    [HttpGet("AddQuestion")]
    public async Task<IActionResult> AddQuestion(string text, Guid filmId)
    {
        var question = new Question
        {
            Id = Guid.NewGuid(),
            FilmId = filmId,
            Text = text
        };

        await _dbContext.Questions.AddAsync(question);
        await _dbContext.SaveChangesAsync();
        
        return RedirectToAction("GetQuestions", new { filmId });
    }

    [HttpGet("Film/{filmId:guid}/Questions")]
    public IActionResult GetQuestions(Guid filmId)
    {
        var film = _dbContext.Films
            .Include(f => f.Questions)
            .FirstOrDefault(f => f.Id == filmId);

        if (film is null)
        {
            return NotFound();
        }

        var random = new Random();

        var viewModel = new FilmQuestionsVm
        {
            FilmId = film.Id,
            FilmName = film.Name,
            Questions = film.Questions
                .AsEnumerable()
                .OrderBy(_ => random.Next())
                .Take(film.QuestionsNumber)
                .ToList()
        };

        return View(viewModel);
    }

}
