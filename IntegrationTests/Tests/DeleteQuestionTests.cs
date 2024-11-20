using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Shouldly;
using Xunit.Priority;
using Xunit;

namespace IntegrationTests.Tests;

public class DeleteQuestionTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();
    private const string BaseUrl = "http://localhost:5000/";

    [Fact, Priority(0)]
    public void Should_DeleteFirstQuestion_When_DeleteQuestion()
    {
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";

        Driver.Navigate().GoToUrl(BaseUrl);
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");

        var editButton = Driver.FindElement(By.CssSelector("a.btn.btn-primary"));
        editButton.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='Name']")));

        var questionsBeforeDelete = Driver.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light.d-flex.justify-content-between.align-items-center.border"));
        int initialQuestionCount = questionsBeforeDelete.Count;
        
        if (initialQuestionCount == 0)
        {
            throw new NoSuchElementException("No questions for deletion.");
        }
        
        var deleteButton = questionsBeforeDelete[0].FindElement(By.CssSelector("a.btn.btn-danger"));
        deleteButton.Click();

        Driver.Navigate().Refresh();

        Console.WriteLine("Waiting for the refreshed 'Edit Film' page to load...");
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='Name']")));

        var updatedQuestions = Driver.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light.d-flex.justify-content-between.align-items-center.border"));
        updatedQuestions.Count.ShouldBe(initialQuestionCount - 1); 
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
