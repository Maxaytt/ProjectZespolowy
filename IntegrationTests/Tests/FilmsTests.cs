using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Shouldly;
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

        public FilmsTests()
        {
            _driver = new EdgeDriver();
        }

        private void Login()
        {
            _driver.Navigate().GoToUrl(LoginUrl);

            _driver.FindElement(By.Id("Email")).SendKeys(Email);
            _driver.FindElement(By.Id("Password")).SendKeys(Password);
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        }

        [Fact, Priority(0)]
        public void Should_Delete_Film()
        {
            // Arrange
            Login();

            // Act
            var filmRow = _driver.FindElements(By.CssSelector(".film-item")).FirstOrDefault();
            filmRow.ShouldNotBeNull("No films found to delete.");

            var filmName = filmRow.FindElement(By.CssSelector(".card .film-name")).Text;

            var deleteButton = filmRow.FindElement(By.CssSelector("#delete-btn"));
            deleteButton.Click();

            // Assert
            var filmsList = _driver.FindElements(By.CssSelector("#film-list"));
            var isFilmDeleted = filmsList.All(element => 
                element.FindElements(By.CssSelector(".film-name")).Count == 0 ||
                element.FindElement(By.CssSelector(".film-name")).Text != filmName);

            isFilmDeleted.ShouldBeTrue("The film was not removed from the list.");
        }
        
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
