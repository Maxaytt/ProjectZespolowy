using E2ETests.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Shouldly;

namespace E2ETests.Tests;

[TestCaseOrderer("E2ETests.Services.PriorityOrderer", "E2ETests")]
public class AnswerTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();
    private const string BaseUrl = "http://localhost:5000/";
    private const string LoginUrl = "http://localhost:5000/Auth/Login";
    private const string Email = "testuser@example.com";
    private const string Password = "Qwer1234!";
    private const string QuestionText = "Test Question";
    private const string AnswerText = "add Answer for TESTS";

[Fact, TestPriority(0)]
public void Should_AddAnswer_When_ValidData()
{
    Login();

    Driver.Navigate().GoToUrl($"{BaseUrl}Home/Index");
    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));

    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").ToString() == "complete");

    var firstFilmEditButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href*='/Films/Edit']")));
    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", firstFilmEditButton);

    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("questionText")));

    var questionInput = Driver.FindElement(By.Id("questionText"));
    questionInput.SendKeys(QuestionText);
    var addQuestionButton = Driver.FindElement(By.Id("question-btn"));
    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", addQuestionButton);

    wait.Until(d => d.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light")).Any());

    var questionElements = Driver.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light"));
    var question = questionElements.FirstOrDefault(q => q.Text.Contains(QuestionText));

    var addAnswerButton = question.FindElement(By.CssSelector("a.btn.btn-success.me-2"));
    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", addAnswerButton);

    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Text")));
    var answerInput = Driver.FindElement(By.Id("Text"));
    answerInput.SendKeys(AnswerText);

    var saveButton = Driver.FindElement(By.CssSelector("button.btn-primary"));
    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", saveButton);

    wait.Until(d => d.FindElements(By.CssSelector("ul.list-group > li.list-group-item")).Any(a => a.Text.Contains(AnswerText)));
}
    private void Login()
    {
        Driver.Navigate().GoToUrl(LoginUrl);
        Driver.FindElement(By.Id("Email")).SendKeys(Email);
        Driver.FindElement(By.Id("Password")).SendKeys(Password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(ExpectedConditions.UrlContains("Home/Index"));
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
