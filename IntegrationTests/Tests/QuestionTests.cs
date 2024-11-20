using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Shouldly;
using Xunit.Priority;
using Xunit;

namespace IntegrationTests.Tests;

public class QuestionTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();
    private const string BaseUrl = "http://localhost:5000/";
    private const string Email = "testuser@example.com";
    private const string Password = "Qwer1234!";

    [Fact, Priority(0)]
    public void Should_DeleteFirstQuestion_When_DeleteQuestion()
    {
        // Arrange
        Driver.Navigate().GoToUrl(BaseUrl);
        Driver.FindElement(By.Id("Email")).SendKeys(Email);
        Driver.FindElement(By.Id("Password")).SendKeys(Password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");

        // Act
        var editButton = Driver.FindElement(By.CssSelector("a.btn.btn-primary"));
        editButton.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='Name']")));

        var questionsBeforeDelete = Driver.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light.d-flex.justify-content-between.align-items-center.border"));
        var initialQuestionCount = questionsBeforeDelete.Count;
        
       initialQuestionCount.ShouldNotBe(0);
        
        var deleteButton = questionsBeforeDelete[0].FindElement(By.CssSelector("a.btn.btn-danger"));
        deleteButton.Click();
        
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='Name']")));
        // Assert
        var updatedQuestions = Driver.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light.d-flex.justify-content-between.align-items-center.border"));
        updatedQuestions.Count.ShouldBe(initialQuestionCount - 1); 
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
