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

    [Fact, Priority(0)]
    public void Should_AddFilm_When_ValidData()
    {
        // Arrange
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";
        const string filmTitle = "Test Film";
       
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
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");
        
        Driver.Navigate().GoToUrl($"{BaseUrl}Films");
        
        wait.Until(d => d.FindElement(By.Id("Name")).Displayed);

        Driver.FindElement(By.Id("Name")).SendKeys(filmTitle);

        Driver.FindElement(By.Name("VideoFile")).SendKeys(videoPath);
        Driver.FindElement(By.Name("ImageFile")).SendKeys(imagePath); 
        
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");
        
        // Assert
        Driver.Url.ShouldBe($"{BaseUrl}Home/Index");
        Driver.FindElement(By.ClassName("film-item")).ShouldNotBeNull();
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
