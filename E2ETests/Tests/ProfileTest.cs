using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Shouldly;
using Xunit.Priority;

namespace IntegrationTests.Tests;

public class ProfileTests : IDisposable
{
    public readonly IWebDriver Driver = new EdgeDriver();

    private const string BaseUrl = "http://localhost:5000/";
    
   [Fact, Priority(0)]
    public void Should_RedirectToLogin_When_EditProfile()
    {   
        // Arrange
        const string email = "testuser@example.com";
        const string password = "Qwer1234!";
        
        // Act
        Driver.Navigate().GoToUrl("http://localhost:5000/");
        
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        Driver.FindElement(By.Id("profilrButton")).Click();
        Driver.FindElement(By.Id("editButton")).Click();
       
       var input1 = Driver.FindElement(By.Id("firstName"));
       var input2 = Driver.FindElement(By.Id("lastName"));
       var input3 = Driver.FindElement(By.Id("email"));

       input1.Clear();
       input1.SendKeys("Alex");

       input2.Clear();
       input2.SendKeys("Fox");

       input3.Clear();
       input3.SendKeys("AlexFox@gmail.com");

       Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

       var successMessage = Driver.FindElement(By.Id("successMessage")); // Замените ID на актуальный
                if (successMessage.Text.Contains("Changes saved successfully"))
                {
                    Console.WriteLine("Test Passed: Changes saved successfully.");
                }
                else
                {
                    Console.WriteLine("Test Failed: Changes were not saved.");
                }

         Dispose();       
    }
        
   
    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
