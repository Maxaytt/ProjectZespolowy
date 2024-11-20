using E2ETests.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Shouldly;

namespace E2ETests.Tests;

[TestCaseOrderer("E2ETests.Services.PriorityOrderer", "E2ETests")]
public class FilmTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();
    private const string BaseUrl = "http://localhost:5000/";
    private const string LoginUrl = "http://localhost:5000/Auth/Login";
    private const string Email = "testuser@example.com";
    private const string Password = "Qwer1234!";
    private const string FilmTitle = "Test Film";
    private readonly string _incompleteVideoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "TestVideo.mp4");
    private readonly string _incompleteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "TestImage.jpg");

    [Fact, TestPriority(0)]
    public void Should_AddFilm_When_ValidData()
    {
        // Arrange
        var videoPath = Path.GetFullPath(_incompleteVideoPath);

        if (!File.Exists(videoPath)) 
        {
            throw new FileNotFoundException($"Film not found: {videoPath}");
        }
        
        var imagePath = Path.GetFullPath(_incompleteImagePath);

        if (!File.Exists(imagePath)) 
        {
            throw new FileNotFoundException($"Image not found: {imagePath}");
        }
        
        // Act
        Driver.Navigate().GoToUrl(BaseUrl);
        Driver.FindElement(By.Id("Email")).SendKeys(Email);
        Driver.FindElement(By.Id("Password")).SendKeys(Password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");
        
        Driver.FindElement(By.Id("create-film")).Click();
        
        wait.Until(d => d.FindElement(By.Id("Name")).Displayed);

        Driver.FindElement(By.Id("Name")).SendKeys(FilmTitle);

        Driver.FindElement(By.Name("VideoFile")).SendKeys(videoPath);
        Driver.FindElement(By.Name("ImageFile")).SendKeys(imagePath); 
        
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");
        
        // Assert
        Driver.Url.ShouldBe($"{BaseUrl}Home/Index");
        Driver.FindElement(By.ClassName("film-item")).ShouldNotBeNull();
    }
    
    [Fact, TestPriority(1)]
    public void Should_UpdateFilm_When_Edit()
    {
        // Arrange
        var uniqueName = Guid.NewGuid().ToString();

        // Act 
        Driver.Navigate().GoToUrl("http://localhost:5000/");
        Driver.FindElement(By.Id("Email")).SendKeys(Email);
        Driver.FindElement(By.Id("Password")).SendKeys(Password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        var firstFilmEditLink = Driver.FindElement(By.Id("edit-href"));
        firstFilmEditLink.Click();
        
        var nameField = Driver.FindElement(By.Id("Name"));
        nameField.Clear();
        nameField.SendKeys(uniqueName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        Driver.Navigate().GoToUrl("http://localhost:5000/Home/Index");
        var updatedFilm = Driver.FindElements(By.XPath($"//*[text()='{uniqueName}']"));
        updatedFilm.Count.ShouldBe(1, "Updated film with unique name was not found.");
    }

    [Fact, TestPriority(2)]
    public void Should_DeleteFilm_When_Delete()
    {
        // Arrange
        Login();

        // Act
        var filmRow = Driver.FindElements(By.CssSelector(".film-item")).FirstOrDefault();
        filmRow.ShouldNotBeNull("No films found to delete.");

        var filmName = filmRow.FindElement(By.CssSelector(".card .film-name")).Text;
        var deleteButton = filmRow.FindElement(By.CssSelector("#delete-btn"));
        deleteButton.Click();

        // Assert
        var filmsList = Driver.FindElements(By.CssSelector("#film-list"));
        var isFilmDeleted = filmsList.All(element => 
            element.FindElements(By.CssSelector(".film-name")).Count == 0 ||
            element.FindElement(By.CssSelector(".film-name")).Text != filmName);

        isFilmDeleted.ShouldBeTrue("The film was not removed from the list.");
    }

    private void Login()
    {
        Driver.Navigate().GoToUrl(LoginUrl);

        Driver.FindElement(By.Id("Email")).SendKeys(Email);
        Driver.FindElement(By.Id("Password")).SendKeys(Password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    }

    private void CreateFilm()
    {
        var videoPath = Path.GetFullPath(_incompleteVideoPath);

        if (!File.Exists(videoPath)) 
        {
            throw new FileNotFoundException($"Film not found: {videoPath}");
        }
        
        var imagePath = Path.GetFullPath(_incompleteImagePath);

        if (!File.Exists(imagePath)) 
        {
            throw new FileNotFoundException($"Image not found: {imagePath}");
        }
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        
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