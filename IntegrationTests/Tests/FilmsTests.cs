using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Shouldly;
using Xunit;
using Xunit.Priority;

namespace IntegrationTests.Tests
{
    public class FilmsTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "http://localhost:5000/";
        private const string LoginUrl = "http://localhost:5000/Auth/Login";
        private const string Email = "testuser@example.com";
        private const string Password = "Qwer1234!";
        private static string? _filmName; 

        public FilmsTests()
        {
            _driver = new EdgeDriver();
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        private void Login()
        {
            _driver.Navigate().GoToUrl(LoginUrl);

            _driver.FindElement(By.Id("Email")).SendKeys(Email);
            _driver.FindElement(By.Id("Password")).SendKeys(Password);
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            _driver.Url.ShouldBe(BaseUrl);
        }

        [Fact, Priority(0)]
        public void Should_Delete_Film()
        {
            // Arrange
            Login();
            _driver.Navigate().GoToUrl($"{BaseUrl}/Films");

            // Act
            var filmRow = _driver.FindElements(By.CssSelector("tr.film-row")).FirstOrDefault();
            filmRow.ShouldNotBeNull("No films found to delete.");

            _filmName = filmRow.FindElement(By.CssSelector(".film-name")).Text;

            var deleteButton = filmRow.FindElement(By.CssSelector(".btn-delete"));
            deleteButton.Click();

            // Assert
            var filmsList = _driver.FindElements(By.CssSelector("tr.film-row"));
            filmsList.ShouldNotContain(f => f.FindElement(By.CssSelector(".film-name")).Text == _filmName,
                "The film was not removed from the list.");
        }
    }
}
