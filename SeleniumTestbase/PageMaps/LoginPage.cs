using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class LoginPage
    {
        private readonly By _username = By.Id("user-name");
        private readonly By _password = By.Id("password");
        private readonly By _loginButton = By.Id("login-button");
    }
}
