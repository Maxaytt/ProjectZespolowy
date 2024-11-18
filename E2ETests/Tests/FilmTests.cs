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
       
        var filmVideoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests", "Assets", "TestVideo.mp4");

        if (!File.Exists(filmVideoPath)) 
        {
            throw new FileNotFoundException($"Film not found: {filmVideoPath}");
        }

        var filmImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests", "Assets", "TestImage.png");

        if (!File.Exists(filmImagePath)) 
        {
            throw new FileNotFoundException($"Image not found: {filmImagePath}");
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

        Driver.FindElement(By.Name("VideoFile")).SendKeys(filmVideoPath);
        Driver.FindElement(By.Name("ImageFile")).SendKeys(filmImagePath); 
        
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");
        
        // Assert
        Driver.Url.ShouldBe($"{BaseUrl}Home/Index");
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
