using OpenQA.Selenium;

namespace SeleniumTestbase
{
    public partial class CheckoutStepOnePage(IWebDriver? driver, BrowserSession? session)
        : BasePage(driver, session)
    {
        public override void WaitToLoad()
        {
            Session?.Wait().Until(_ => GetElement(_header) is { Displayed: true, Text: "Checkout: Your Information" });
        }

        /// <summary>
        /// Returns the page title text (e.g. "Checkout: Your Information").
        /// </summary>
        public string GetHeaderText()
        {
            return GetText(_header);
        }

        /// <summary>
        /// Fills in the first name field.
        /// </summary>
        public void EnterFirstName(string firstName)
        {
            Log.Step($"Entering first name: '{firstName}'");
            GetElement(_firstNameInput)?.SendKeys(firstName);
        }

        /// <summary>
        /// Fills in the last name field.
        /// </summary>
        public void EnterLastName(string lastName)
        {
            Log.Step($"Entering last name: '{lastName}'");
            GetElement(_lastNameInput)?.SendKeys(lastName);
        }

        /// <summary>
        /// Fills in the postal code field.
        /// </summary>
        public void EnterPostalCode(string postalCode)
        {
            Log.Step($"Entering postal code: '{postalCode}'");
            GetElement(_postalCodeInput)?.SendKeys(postalCode);
        }

        /// <summary>
        /// Fills in all checkout information fields at once.
        /// </summary>
        public void FillForm(string firstName, string lastName, string postalCode)
        {
            Log.Step($"Filling checkout form: {firstName} {lastName}, {postalCode}");
            EnterFirstName(firstName);
            EnterLastName(lastName);
            EnterPostalCode(postalCode);
        }

        /// <summary>
        /// Clicks "Continue" to proceed to checkout step two.
        /// </summary>
        public void Continue()
        {
            Log.Step("Clicking Continue");
            GetElement(_continueButton)?.Click();
        }

        /// <summary>
        /// Clicks "Cancel" to navigate back to the cart page.
        /// </summary>
        public void Cancel()
        {
            Log.Step("Clicking Cancel");
            GetElement(_cancelButton)?.Click();
        }

        /// <summary>
        /// Fills the form and clicks Continue in one step.
        /// </summary>
        public void SubmitForm(string firstName, string lastName, string postalCode)
        {
            FillForm(firstName, lastName, postalCode);
            Continue();
        }

        /// <summary>
        /// Returns true if the error message is displayed.
        /// </summary>
        public bool IsErrorDisplayed()
        {
            return WaitForElement(_errorMessage, 3000);
        }

        /// <summary>
        /// Returns the error message text, or empty string if not visible.
        /// </summary>
        public string GetErrorText()
        {
            return GetText(_errorMessage);
        }

        /// <summary>
        /// Returns the current value in the first name field.
        /// </summary>
        public string GetFirstNameValue()
        {
            return GetAttribute(_firstNameInput, "value");
        }

        /// <summary>
        /// Returns the current value in the last name field.
        /// </summary>
        public string GetLastNameValue()
        {
            return GetAttribute(_lastNameInput, "value");
        }

        /// <summary>
        /// Returns the current value in the postal code field.
        /// </summary>
        public string GetPostalCodeValue()
        {
            return GetAttribute(_postalCodeInput, "value");
        }
    }
}