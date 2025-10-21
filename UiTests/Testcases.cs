using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumTestbase;
using Assert = NUnit.Framework.Assert;

namespace UiTests
{
    [TestFixture(BrowserType.Chrome)]
    [TestFixture(BrowserType.Firefox)]
    [TestFixture(BrowserType.Edge)]
    public class Testcases(BrowserType browser) : TestBase(browser)
    {
        [TestCase]
        public void StandardUsersCanLogin()
        {
            IWebDriver? driver = Session?.Driver; ;

            LoginPage loginPage = new LoginPage(driver, Session);

            loginPage.WaitToLoad();
            loginPage.Login(UserType.Locked);

        }
    }
}
