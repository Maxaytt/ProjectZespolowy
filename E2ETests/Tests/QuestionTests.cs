using E2ETests.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Shouldly;

namespace E2ETests.Tests;

[TestCaseOrderer("E2ETests.Services.PriorityOrderer", "E2ETests")]
public class QuestionTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();
    private const string BaseUrl = "http://localhost:5000/";
    private const string LoginUrl = "http://localhost:5000/Auth/Login";
    private const string Email = "testuser@example.com";
    private const string Password = "Qwer1234!";
    private const string FilmTitle = "Test Film";
    private const string QuestionText = "question text";
    private readonly string _incompleteVideoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "TestVideo.mp4");
    private readonly string _incompleteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "TestImage.jpg");
    
    [Fact, TestPriority(0)]
    public void Should_SaveQuestion_When_AddQuestion()
    {
        // Arrange
        var videoPath = Path.GetFullPath(_incompleteVideoPath);
        if (!File.Exists(videoPath)) 
            throw new FileNotFoundException($"Film not found: {videoPath}");
        
        var imagePath = Path.GetFullPath(_incompleteImagePath);
        if (!File.Exists(imagePath)) 
            throw new FileNotFoundException($"Image not found: {imagePath}");
        
        // Act
        Login();
        CreateFilm(videoPath, imagePath);
        Driver.FindElement(By.Id("edit-href")).Click();

        Driver.FindElement(By.Id("questionText")).SendKeys(QuestionText);
        Driver.FindElement(By.Id("question-btn")).Click();
        
        // Assert
        var questionList = Driver.FindElement(By.Id("question-list"));
        var questions = questionList.FindElements(By.CssSelector(".rounded span"));
        
        questions.Any(q => q.Text == QuestionText).ShouldBeTrue();
    }
    
    private void Login()
    {
        Driver.Navigate().GoToUrl(LoginUrl);

        Driver.FindElement(By.Id("Email")).SendKeys(Email);
        Driver.FindElement(By.Id("Password")).SendKeys(Password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    }

    private void CreateFilm(string videoPath, string imagePath)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");
        
        Driver.FindElement(By.Id("create-film")).Click();
        
        wait.Until(d => d.FindElement(By.Id("Name")).Displayed);

        Driver.FindElement(By.Id("Name")).SendKeys(FilmTitle);

        Driver.FindElement(By.Name("VideoFile")).SendKeys(videoPath);
        Driver.FindElement(By.Name("ImageFile")).SendKeys(imagePath); 
        
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");
    }
    
    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}