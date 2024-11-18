using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Shouldly;
using Xunit.Priority;

namespace E2ETests.Tests;

public class FilmTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();
    private const string BaseUrl = "http://localhost:5000/";
    private const string LoginUrl = "http://localhost:5000/Auth/Login";
    private const string Email = "testuser@example.com";
    private const string Password = "Qwer1234!";
    private const string FilmTitle = "Test Film";

    [Fact, Priority(0)]
    public void Should_AddFilm_When_ValidData()
    {
        // Arrange
        var incompleteVideoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "TestVideo.mp4");
        var videoPath = Path.GetFullPath(incompleteVideoPath);

        if (!File.Exists(videoPath)) 
        {
            throw new FileNotFoundException($"Film not found: {videoPath}");
        }

        var incompleteImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "TestImage.jpg");
        var imagePath = Path.GetFullPath(incompleteImagePath);

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
        
        Driver.Navigate().GoToUrl($"{BaseUrl}Films");
        
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
    
    [Fact, Priority(1)]
    public void Should_UpdateFilm_When_Edit()
    {
        // Arrange
        var uniqueName = Guid.NewGuid().ToString();

        // Act 
        Driver.Navigate().GoToUrl("http://localhost:5000/");
        Driver.FindElement(By.Id("Email")).SendKeys(Email);
        Driver.FindElement(By.Id("Password")).SendKeys(Password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        var firstFilmEditLink = Driver.FindElement(By.CssSelector("a.btn.btn-primary[href*='/Films/Edit']"));
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

    [Fact, Priority(2)]
    public void Should_Delete_Film()
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
    
    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}