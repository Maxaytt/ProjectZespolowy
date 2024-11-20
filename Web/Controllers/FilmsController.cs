using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Domain.ViewModels;
using Microsoft.EntityFrameworkCore;

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


    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost("Create")]
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
            Questions = film.Questions,
            QuestionsNumber = film.QuestionsNumber
        };
        return View(viewModel);
    }
    
    [HttpPost("Edit")]
    public IActionResult EditPost(CreateEditFilmVm viewModel)
    {
        var existingFilm = _dbContext.Films
            .Include(f => f.Image)
            .Include(f => f.Questions)
            .First(f => f.Id == viewModel.Id);

        if (viewModel.Name is not null)
        {
            existingFilm.Name = viewModel.Name;
            existingFilm.Image.Caption = viewModel.Name;
        }

        if (viewModel.ImageFile is not null)
        {
            using var item = new MemoryStream();
            viewModel.ImageFile.CopyTo(item);
            existingFilm.Image.Content = item.ToArray();
        }

        if (viewModel.VideoFile is not null)
        {
            using var item = new MemoryStream();
            viewModel.VideoFile.CopyTo(item);
            existingFilm.Content = item.ToArray();
        }

        existingFilm.QuestionsNumber = viewModel.QuestionsNumber;
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
        
        return RedirectToAction("Edit", new { id = filmId});
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

    [HttpGet("AddAnswer")]
    public IActionResult AddAnswer(Guid questionId)
    {
        var question = _dbContext.Questions
            .Include(q => q.Answers)
            .Include(q => q.Film)
            .FirstOrDefault(q => q.Id == questionId);

        if (question == null)
            return NotFound("Question not found");

        var viewModel = new AddAnswerVm
        {
            QuestionId = questionId,
            QuestionText = question.Text,
            
            Answers = question.Answers
                .Select(a => (a.Id, a.Text, a.IsTrue))
                .ToList()
        };

        ViewData["FilmId"] = question.FilmId;

        return View(viewModel);
    }
    
    [HttpPost("AddAnswer")]
    [ValidateAntiForgeryToken]
    public IActionResult AddAnswer(AddAnswerVm viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        if (viewModel.IsTrue)
        {
            var existingAnswers = _dbContext.Answers.Where(a => a.QuestionId == viewModel.QuestionId).ToList();
            foreach (var answer in existingAnswers)
            {
                answer.IsTrue = false;
            }
            _dbContext.Answers.UpdateRange(existingAnswers);
        }

        var newAnswer = new Answer
        {
            Id = Guid.NewGuid(),
            QuestionId = viewModel.QuestionId,
            Text = viewModel.Text,
            IsTrue = viewModel.IsTrue
        };

        _dbContext.Answers.Add(newAnswer);
        _dbContext.SaveChanges();

        return RedirectToAction("AddAnswer", new { questionId = viewModel.QuestionId });
    }

    [HttpPost("DeleteAnswer")]
    public IActionResult DeleteAnswer(Guid id, Guid questionId)
    {
        var answer = _dbContext.Answers.Find(id);
        if (answer == null)
            return NotFound();

        _dbContext.Answers.Remove(answer);
        _dbContext.SaveChanges();

        return RedirectToAction("AddAnswer", new { questionId = questionId });
    }
}
