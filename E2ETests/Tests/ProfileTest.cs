using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Shouldly;
using Xunit.Priority;

namespace E2ETests.Tests;

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
        Driver.Navigate().GoToUrl(BaseUrl);
        
        Driver.FindElement(By.Id("Email")).SendKeys(email);
        Driver.FindElement(By.Id("Password")).SendKeys(password);
        
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        Driver.FindElement(By.Id("profileButton")).Click();
        Driver.FindElement(By.Id("editButton")).Click();

        Driver.Url.ShouldBe("http://localhost:5000/Profile/Edit");
       
       var input_name = Driver.FindElement(By.Id("firstName"));
       var input_lastname = Driver.FindElement(By.Id("lastName"));
       var input_email = Driver.FindElement(By.Id("email"));

       input_name.Clear();
       input_name.SendKeys("Alex");

       input_lastname.Clear();
       input_lastname.SendKeys("Fox");

       inputEmail.Clear();
       inputEmail.SendKeys("AlexFox@gmail.com");

       Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

      
       Driver.Url.ShouldBe("http://localhost:5000/Profile/Index");     
    }
        
   
    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}
