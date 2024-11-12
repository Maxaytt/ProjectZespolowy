using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Shouldly;
using Xunit.Priority;

namespace IntegrationTests.Tests;

public class AuthTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();

    private const string BaseUrl = "http://localhost:5000/";
    
    [Fact, Priority(0)]
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
    
    [Fact, Priority(1)]
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
    
    [Fact, Priority(2)]
    public void Can_Edit_Film()
    {
        // Arrange
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";
        const string initialName = "Test";
        const string updatedName = "UpdateName";

        // Act
        Driver.Navigate().GoToUrl("http://localhost:5000/");
    
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Update the name to 'Test' and submit again
        Driver.Navigate().GoToUrl("http://localhost:5000/Films/Edit/52f059d1-e00d-467c-a0f3-7d1adbdd9984");
        Driver.FindElement(By.Id("Name")).Clear();
        Driver.FindElement(By.Id("Name")).SendKeys(updatedName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Re-navigate to verify the final update
        Driver.Navigate().GoToUrl("http://localhost:5000/Films/Edit/52f059d1-e00d-467c-a0f3-7d1adbdd9984");
        Assert.Equal(updatedName, Driver.FindElement(By.Id("Name")).GetAttribute("value"));
        
        Driver.Navigate().GoToUrl("http://localhost:5000/Films/Edit/52f059d1-e00d-467c-a0f3-7d1adbdd9984");
        Driver.FindElement(By.Id("Name")).Clear();
        Driver.FindElement(By.Id("Name")).SendKeys(initialName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
