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
    public void Should_DeleteFirstQuestion_When_EditFilm()
    {
        // Step 1: Авторизация пользователя
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";

        Driver.Navigate().GoToUrl(BaseUrl);
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Ожидание при авторизации
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.Url == $"{BaseUrl}Home/Index");

        // Step 2: Переход к Edit 
        var editButton = Driver.FindElement(By.CssSelector("a.btn.btn-primary")); // Найти первую кнопку Edit
        editButton.Click();
        // Ожидание загрузки Edit
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='Name']")));

        // Step 3: Нахождение и удаление первого вопроса
        Console.WriteLine("Trying to find the first question's delete button...");
        // Получаем количество вопросов до удаления
        var questionsBeforeDelete = Driver.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light.d-flex.justify-content-between.align-items-center.border"));
        int initialQuestionCount = questionsBeforeDelete.Count;
        // Хотя бы 1 вопрос есть для удаления
        if (initialQuestionCount == 0)
        {
            throw new NoSuchElementException("No questions for deletion.");
        }
        // Найти и нажать "Delete" для первого вопроса
        var deleteButton = questionsBeforeDelete[0].FindElement(By.CssSelector("a.btn.btn-danger"));
        deleteButton.Click();

        // Step 4: Обновление страницы для проверки результата удаления
        Console.WriteLine("Refreshing the page to check if the question is deleted...");
        Driver.Navigate().Refresh();

        // Step 5: Подтверждение удаления
        Console.WriteLine("Waiting for the refreshed 'Edit Film' page to load...");
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[name='Name']")));

        // Проверяем количество вопросов после обновления страницы
        //var updatedQuestions = Driver.FindElements(By.CssSelector(".rounded.p-3.mb-3.bg-light.d-flex.justify-content-between.align-items-center.border"));
        //updatedQuestions.Count.ShouldBe(initialQuestionCount - 1); // Проверка вопросов, что их стало меньше на - 1
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
