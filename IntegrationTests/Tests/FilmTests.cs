using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Shouldly;
using Xunit.Priority;

namespace IntegrationTests.Tests;

public class FilmTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();
    
    [Fact, Priority(0)]
    public void Should_UpdateFilm_When_Edit()
    {
        // Arrange
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";
        var uniqueName = Guid.NewGuid().ToString();

        // Act 
        Driver.Navigate().GoToUrl("http://localhost:5000/");
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
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
        Assert.True(updatedFilm.Count > 0, "Updated film with unique name was not found.");
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}