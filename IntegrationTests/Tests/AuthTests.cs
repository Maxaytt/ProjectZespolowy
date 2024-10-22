using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Shouldly;

namespace IntegrationTests.Tests;

public class AuthTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();

    private const string BaseUrl = "http://localhost:5000/";
    
    [Fact]
    public void Should_RedirectOrConflict_When_Register()
    {
        // Arrange
        const string firstName = "Test";
        const string lastName = "User";
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";

        // Act
        Driver.Navigate().GoToUrl("http://localhost:5000/Auth/Register");

        Driver.FindElement(By.Id("FirstName")).SendKeys(firstName);
        Driver.FindElement(By.Id("LastName")).SendKeys(lastName);
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
    
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    
        // Assert
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));

        var isRedirected = Driver.Url == BaseUrl;
        if (isRedirected)
        {
            Driver.Url.ShouldBe(BaseUrl); 
        }
        else
        {
            wait.Until(d => d.FindElement(By.TagName("pre")).Displayed);
            var errorMessage = Driver.FindElement(By.TagName("pre")).Text;
            errorMessage.ShouldContain("already exists");
        }
    }
    
    [Fact]
    public void Should_Redirect_When_Login()
    {
        // Arrange
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";
        
        // Act
        Driver.Navigate().GoToUrl("http://localhost:5000/");
        
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        
        // Assert
        Driver.Url.ShouldBe("http://localhost:5000/Home/Index");
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
