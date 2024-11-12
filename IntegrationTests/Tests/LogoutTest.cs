using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;  // Добавьте эту строку
using Shouldly;
using Xunit.Priority;

namespace IntegrationTests.Tests;

public class LogoutTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();

    private const string BaseUrl = "http://localhost:5000";

    [Fact, Priority(0)]
    public void Should_RedirectToLogin_When_Logout()
    {
        // Arrange
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";

        // Act - Выполняем логин перед логаутом
        Driver.Navigate().GoToUrl(BaseUrl);
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Убедимся, что пользователь попал на страницу Home/Index
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");

        // Act - Выполняем логаут
        var logoutButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("logoutButton")));
        logoutButton.Click();

        // Assert - Проверяем, что после выхода происходит перенаправление на страницу логина
        wait.Until(d => d.Url == $"{BaseUrl}Auth/Login");
        Driver.Url.ShouldBe($"{BaseUrl}Auth/Login");
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
