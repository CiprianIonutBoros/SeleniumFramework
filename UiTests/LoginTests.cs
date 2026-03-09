using NUnit.Framework;
using SeleniumTestbase;
using Assert = NUnit.Framework.Assert;

namespace UiTests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Children)]
    public class LoginTests : TestBase
    {
        [Test, Category("Smoke")]
        public void StandardUserCanLogin()
        {
            Pages.Login.WaitToLoad();
            Pages.Login.Login(UserType.Standard);

            Assert.That(Pages.Login.IsInventoryPageDisplayed(), Is.True,
                "Standard user should be redirected to the inventory page after login.");
        }

        [Test, Category("Smoke")]
        public void LockedUserCannotLogin()
        {
            Pages.Login.WaitToLoad();
            Pages.Login.Login(UserType.Locked);

            Assert.That(Pages.Login.IsErrorMessageDisplayed(), Is.True,
                "Error message should be displayed for locked user.");
            Assert.That(Pages.Login.GetErrorMessageText(), Does.Contain("locked out"),
                "Error message should indicate the user is locked out.");
        }

        [Test, Category("Regression")]
        public void ProblematicUserCanLogin()
        {
            Pages.Login.WaitToLoad();
            Pages.Login.Login(UserType.Problematic);

            Assert.That(Pages.Login.IsInventoryPageDisplayed(), Is.True,
                "Problematic user should be redirected to the inventory page after login.");
        }

        [Test, Category("Regression")]
        public void GlitchUserCanLogin()
        {
            Pages.Login.WaitToLoad();
            Pages.Login.Login(UserType.Glitch);

            Assert.That(Pages.Login.IsInventoryPageDisplayed(), Is.True,
                "Glitch user should be redirected to the inventory page after login.");
        }

        [Test, Category("Regression")]
        public void ErrorUserCanLogin()
        {
            Pages.Login.WaitToLoad();
            Pages.Login.Login(UserType.Error);

            Assert.That(Pages.Login.IsInventoryPageDisplayed(), Is.True,
                "Error user should be redirected to the inventory page after login.");
        }

        [Test, Category("Regression")]
        public void VisualUserCanLogin()
        {
            Pages.Login.WaitToLoad();
            Pages.Login.Login(UserType.Visual);

            Assert.That(Pages.Login.IsInventoryPageDisplayed(), Is.True,
                "Visual user should be redirected to the inventory page after login.");
        }
    }
}