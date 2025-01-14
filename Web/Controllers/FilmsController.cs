using System.Security.Claims;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace Web.Controllers;

/// <summary>
/// Controller responsible for managing films in the application.
/// </summary>
[Route("[controller]")]
public class FilmsController : Controller
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Constructs the controller with dependency injection for the database context.
    /// </summary>
    /// <param name="dbContext">The database context for the application.</param>
    public FilmsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Retrieves a film by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the film.</param>
    /// <returns>The film with the specified ID, or a 404 error if not found.</returns>
    [HttpGet("{id:guid}")]
    [Authorize]
    public IActionResult GetById(Guid id)
    {
        var film = _dbContext.Films.Find(id);
        if (film == null)
            return NotFound();

        return Ok(film);
    }

    /// <summary>
    /// Displays the page for creating a new film.
    /// </summary>
    /// <returns>The view for creating a new film.</returns>
    [HttpGet("Create")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handles the creation of a new film.
    /// </summary>
    /// <param name="film">The view model for creating a new film.</param>
    /// <returns>Redirects to the home page after successfully creating the film.</returns>
    [HttpPost("Create")]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Displays the page for editing an existing film.
    /// </summary>
    /// <param name="id">The identifier of the film to edit.</param>
    /// <returns>The view for editing the film.</returns>
    [HttpGet("Edit/{id:guid}")]
    [Authorize(Roles = "Admin")]
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
    
    /// <summary>
    /// Handles the post request for editing an existing film.
    /// </summary>
    /// <param name="viewModel">The updated view model for the film.</param>
    /// <returns>Redirects to the home page after updating the film.</returns>
    [HttpPost("Edit")]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Deletes a film based on its identifier.
    /// </summary>
    /// <param name="id">The identifier of the film to delete.</param>
    /// <returns>Redirects to the home page after deleting the film.</returns>
    [HttpGet("Delete")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(Guid id)
    {
        var film = _dbContext.Films.Find(id);
        if (film is null)
            return NotFound();

        _dbContext.Films.Remove(film);
        _dbContext.SaveChanges();

        return RedirectToAction(nameof(Index), "Home");
    }

    /// <summary>
    /// Retrieves a film as a downloadable resource.
    /// </summary>
    /// <param name="id">The identifier of the film to retrieve.</param>
    /// <returns>The film content or a 404 error if not found.</returns>
    [HttpGet("GetFilmAsResource/{id:guid}")]
    [Authorize]
    public IActionResult GetFilmAsResource(Guid id)
    {
        var film = _dbContext.Films.Find(id);
        if (film is null)
            return NotFound();
        return File(film.Content, film.ContentType, film.Name);
    }

    /// <summary>
    /// Retrieves an image associated with a film as a downloadable resource.
    /// </summary>
    /// <param name="id">The identifier of the image to retrieve.</param>
    /// <returns>The image content or a 404 error if not found.</returns>
    [HttpGet("GetImageAsResource/{id:guid}")]
    [Authorize]
    public IActionResult GetImageAsResource(Guid id)
    {
        var image = _dbContext.Images.Find(id);
        if (image is null)
            return NotFound();
        return File(image.Content, image.ContentType, image.Caption);
    }

    /// <summary>
    /// Displays the page for playing a film.
    /// </summary>
    /// <param name="id">The identifier of the film to play.</param>
    /// <returns>The view for playing the film.</returns>
    [HttpGet("PlayFilm/{id:guid}")]
    [Authorize]
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
    
    /// <summary>
    /// Deletes a question associated with a film.
    /// </summary>
    /// <param name="id">The identifier of the question to delete.</param>
    /// <returns>Redirects to the referer page or to the home page after deletion.</returns>
    [HttpGet("DeleteQuestion")]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Adds a new question to a film.
    /// </summary>
    /// <param name="text">The text of the question.</param>
    /// <param name="filmId">The identifier of the film the question belongs to.</param>
    /// <returns>Redirects to the film edit page after adding the question.</returns>
    [HttpGet("AddQuestion")]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Retrieves and submits answers for questions associated with a film.
    /// </summary>
    /// <param name="filmId">The identifier of the film for which the questions are submitted.</param>
    /// <param name="selectedAnswers">The selected answers provided by the user.</param>
    /// <returns>The view displaying the questions and the user's test results.</returns>
    [HttpGet("Film/{filmId:guid}/Questions")]
    [HttpPost("Film/{filmId:guid}/Questions")]
    [Authorize]
    public async Task<IActionResult> GetQuestions(Guid filmId, [FromForm] List<Guid>? selectedAnswers)
    {
        var film = _dbContext.Films
            .Include(f => f.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefault(f => f.Id == filmId);

        if (film == null)
        {
            return NotFound();
        }

        var questions = film.Questions.Take(film.QuestionsNumber).ToList();
        int correctAnswers = 0;
        
        if (selectedAnswers is not null && selectedAnswers.Any())
        {
            var user = await GetUser();
            var testResult = new TestResult
            {
                Id = Guid.NewGuid(),
                UserId = user.Id, 
                FilmId = filmId,  
                Timestamp = DateTime.UtcNow,
                Answers = []
            };
            foreach (var answer in selectedAnswers
                         .Select(answerId => _dbContext.Answers.FirstOrDefault(a => a.Id == answerId)))
            {
                if (answer is { IsTrue: true })
                {
                    correctAnswers++;
                }
                if (answer is not null)
                    testResult.Answers.Add(answer);
            }
            
            _dbContext.TestResults.Add(testResult);
            await _dbContext.SaveChangesAsync();

            ViewBag.TestCompleted = true;
            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.TotalQuestions = questions.Count;
        }
        else
        {
            ViewBag.TestCompleted = false;
        }

        var viewModel = new FilmQuestionsVm
        {
            FilmId = film.Id,
            FilmName = film.Name,
            Questions = questions
        };

        return View(viewModel);
    }
    
    /// <summary>
    /// Displays the page for adding a new answer to a specific question.
    /// </summary>
    /// <param name="questionId">The identifier of the question for which to add an answer.</param>
    /// <returns>The view for adding answers to the specified question.</returns>
    [HttpGet("AddAnswer")]
    [Authorize(Roles = "Admin")]
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
    
    /// <summary>
    /// Handles the creation of a new answer for a specific question.
    /// </summary>
    /// <param name="viewModel">The view model containing the details of the new answer.</param>
    /// <returns>Redirects to the AddAnswer page to display the newly added answer.</returns>
    [HttpPost("AddAnswer")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Deletes an answer from the database.
    /// </summary>
    /// <param name="id">The identifier of the answer to delete.</param>
    /// <param name="questionId">The identifier of the question to which the answer belongs.</param>
    /// <returns>Redirects to the AddAnswer page for the specified question after deletion.</returns>
    [HttpPost("DeleteAnswer")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteAnswer(Guid id, Guid questionId)
    {
        var answer = _dbContext.Answers.Find(id);
        if (answer == null)
            return NotFound();

        _dbContext.Answers.Remove(answer);
        _dbContext.SaveChanges();

        return RedirectToAction("AddAnswer", new { questionId = questionId });
    }

    /// <summary>
    /// Retrieves the current user based on the claims in the HTTP context.
    /// </summary>
    /// <returns>The user associated with the current HTTP context.</returns>
    /// <exception cref="BadHttpRequestException">Thrown if the user ID format is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown if the user is not found in the database.</exception>
    private async Task<User> GetUser()
    {
        var userIdClaim = HttpContext.User.Claims
            .ToList()
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim?.Value, out var userId))
        {
            throw new BadHttpRequestException("Invalid user ID format.");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            throw new NotFoundException($"User with ID '{userId}' not found.");
        }

        return user;
    }
}