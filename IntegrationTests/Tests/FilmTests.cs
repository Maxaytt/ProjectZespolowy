using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Shouldly;
using Xunit;
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
        const string filmVideoPath = @"C:\Users\Acer\Downloads\Как капибары принимают горячую ванну в японском зоопарке.mp4"; 
        const string filmImagePath = @"C:\Users\Acer\Pictures\Day\image.jpg";

        // Step 1: Авторизация пользователя
        Driver.Navigate().GoToUrl(BaseUrl);
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Ожидание перенаправления на главную страницу после авторизации
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");

        // Step 2: Переход на страницу добавления фильма
        Driver.Navigate().GoToUrl($"{BaseUrl}Films");

        // Ожидание, пока элемент с ID "Name" не станет доступным на странице добавления фильма
        wait.Until(d => d.FindElement(By.Id("Name")).Displayed);

        // Step 3: Заполнение формы добавления фильма
        Driver.FindElement(By.Id("Name")).SendKeys(filmTitle);

        // Используем name атрибуты для загрузки файлов
        Driver.FindElement(By.Name("VideoFile")).SendKeys(filmVideoPath); // Поле для загрузки видео
        Driver.FindElement(By.Name("ImageFile")).SendKeys(filmImagePath); // Поле для загрузки изображения

        // Отправка формы
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Step 4: Ожидание перенаправления на главную страницу после добавления фильма
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");

        // Проверка, что перенаправление произошло на главную страницу
        Driver.Url.ShouldBe($"{BaseUrl}Home/Index");
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
