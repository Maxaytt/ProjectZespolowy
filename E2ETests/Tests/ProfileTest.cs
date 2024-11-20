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
    public void Should_UpdateUser_When_EditProfile()
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
       
       var inputName = Driver.FindElement(By.Id("firstName"));
       var inputLastname = Driver.FindElement(By.Id("lastName"));
       var inputEmail = Driver.FindElement(By.Id("email"));

       inputName.Clear();
       inputName.SendKeys("Alex");

       inputLastname.Clear();
       inputLastname.SendKeys("Fox");

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
