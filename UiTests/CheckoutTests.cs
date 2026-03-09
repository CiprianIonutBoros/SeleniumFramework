using NUnit.Framework;
using SeleniumTestbase;
using Assert = NUnit.Framework.Assert;

namespace UiTests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Children)]
    public class CheckoutTests : TestBase
    {
        // ── Step One ─────────────────────────────────────────────────

        [Test, Category("Smoke")]
        public void CheckoutStepOneDisplaysCorrectHeader()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutStepOne.GetHeaderText(), Is.EqualTo("Checkout: Your Information"),
                "Checkout step one header should be 'Checkout: Your Information'.");
        }

        [Test, Category("Smoke")]
        public void SubmitWithValidInfoNavigatesToStepTwo()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Pages.CheckoutStepTwo.WaitToLoad();

            Assert.That(Pages.CheckoutStepTwo.GetHeaderText(), Is.EqualTo("Checkout: Overview"),
                "Should navigate to checkout step two after valid submission.");
        }

        [Test, Category("Regression")]
        public void SubmitWithEmptyFirstNameShowsError()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.SubmitForm("", TestData.Checkout.LastName, TestData.Checkout.PostalCode);

            Assert.That(Pages.CheckoutStepOne.IsErrorDisplayed(), Is.True,
                "Error should be displayed when first name is empty.");
            Assert.That(Pages.CheckoutStepOne.GetErrorText(), Does.Contain("First Name"),
                "Error message should mention First Name.");
        }

        [Test, Category("Regression")]
        public void SubmitWithEmptyLastNameShowsError()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.SubmitForm(TestData.Checkout.FirstName, "", TestData.Checkout.PostalCode);

            Assert.That(Pages.CheckoutStepOne.IsErrorDisplayed(), Is.True,
                "Error should be displayed when last name is empty.");
            Assert.That(Pages.CheckoutStepOne.GetErrorText(), Does.Contain("Last Name"),
                "Error message should mention Last Name.");
        }

        [Test, Category("Regression")]
        public void SubmitWithEmptyPostalCodeShowsError()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.SubmitForm(TestData.Checkout.FirstName, TestData.Checkout.LastName, "");

            Assert.That(Pages.CheckoutStepOne.IsErrorDisplayed(), Is.True,
                "Error should be displayed when postal code is empty.");
            Assert.That(Pages.CheckoutStepOne.GetErrorText(), Does.Contain("Postal Code"),
                "Error message should mention Postal Code.");
        }

        [Test, Category("Regression")]
        public void SubmitWithAllFieldsEmptyShowsError()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.Continue();

            Assert.That(Pages.CheckoutStepOne.IsErrorDisplayed(), Is.True,
                "Error should be displayed when all fields are empty.");
        }

        [Test, Category("Regression")]
        public void CheckoutStepOneCancelReturnsToCart()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.Cancel();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.GetHeaderText(), Is.EqualTo("Your Cart"),
                "Cancel should navigate back to the cart page.");
        }

        [Test, Category("Regression")]
        public void CheckoutStepOneFormFieldsAcceptInput()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.FillForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Assert.That(Pages.CheckoutStepOne.GetFirstNameValue(), Is.EqualTo(TestData.Checkout.FirstName),
                "First name field should contain the entered value.");
            Assert.That(Pages.CheckoutStepOne.GetLastNameValue(), Is.EqualTo(TestData.Checkout.LastName),
                "Last name field should contain the entered value.");
            Assert.That(Pages.CheckoutStepOne.GetPostalCodeValue(), Is.EqualTo(TestData.Checkout.PostalCode),
                "Postal code field should contain the entered value.");
        }

        // ── Step Two ─────────────────────────────────────────────────

        [Test, Category("Smoke")]
        public void CheckoutStepTwoDisplaysCorrectHeader()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutStepTwo.GetHeaderText(), Is.EqualTo("Checkout: Overview"),
                "Checkout step two header should be 'Checkout: Overview'.");
        }

        [Test, Category("Smoke")]
        public void SingleItemAppearsInOverview()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutStepTwo.GetItemCount(), Is.EqualTo(1),
                "Overview should show 1 item.");
            Assert.That(Pages.CheckoutStepTwo.IsItemInSummary(TestData.Products.Backpack), Is.True,
                $"{TestData.Products.Backpack} should appear in the checkout overview.");
        }

        [Test, Category("Regression")]
        public void MultipleItemsAppearInOverview()
        {
            NavigateToCheckoutStepTwo(
                TestData.Products.Backpack,
                TestData.Products.BikeLight,
                TestData.Products.Onesie);

            Assert.That(Pages.CheckoutStepTwo.GetItemCount(), Is.EqualTo(3),
                "Overview should show 3 items.");
            Assert.That(Pages.CheckoutStepTwo.IsItemInSummary(TestData.Products.Backpack), Is.True);
            Assert.That(Pages.CheckoutStepTwo.IsItemInSummary(TestData.Products.BikeLight), Is.True);
            Assert.That(Pages.CheckoutStepTwo.IsItemInSummary(TestData.Products.Onesie), Is.True);
        }

        [Test, Category("Regression")]
        public void OverviewItemPricesStartWithDollarSign()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack, TestData.Products.FleeceJacket);

            List<string> prices = Pages.CheckoutStepTwo.GetItemPrices();

            Assert.That(prices, Has.Count.EqualTo(2),
                "Overview should display prices for both items.");
            Assert.That(prices, Has.All.StartsWith("$"),
                "All item prices should start with a dollar sign.");
        }

        [Test, Category("Regression")]
        public void OverviewItemQuantitiesAreOne()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack, TestData.Products.BikeLight);

            Assert.That(Pages.CheckoutStepTwo.GetItemQuantities(), Has.All.EqualTo(1),
                "Each item quantity should be 1.");
        }

        [Test, Category("Regression")]
        public void PaymentInfoIsDisplayed()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutStepTwo.GetPaymentInfo(), Is.Not.Empty,
                "Payment information should be displayed.");
            Assert.That(Pages.CheckoutStepTwo.GetPaymentInfo(), Does.Contain("SauceCard"),
                "Payment should reference SauceCard.");
        }

        [Test, Category("Regression")]
        public void ShippingInfoIsDisplayed()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutStepTwo.GetShippingInfo(), Is.Not.Empty,
                "Shipping information should be displayed.");
            Assert.That(Pages.CheckoutStepTwo.GetShippingInfo(), Does.Contain("Pony Express"),
                "Shipping should reference Pony Express.");
        }

        [Test, Category("Smoke")]
        public void SubtotalIsGreaterThanZero()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutStepTwo.GetSubtotalAmount(), Is.GreaterThan(0),
                "Subtotal should be greater than zero.");
        }

        [Test, Category("Regression")]
        public void TaxIsGreaterThanZero()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutStepTwo.GetTaxAmount(), Is.GreaterThan(0),
                "Tax should be greater than zero.");
        }

        [Test, Category("Regression")]
        public void TotalEqualsSubtotalPlusTax()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            decimal subtotal = Pages.CheckoutStepTwo.GetSubtotalAmount();
            decimal tax = Pages.CheckoutStepTwo.GetTaxAmount();
            decimal total = Pages.CheckoutStepTwo.GetTotalAmount();

            Assert.That(total, Is.EqualTo(subtotal + tax).Within(0.01m),
                "Total should equal subtotal + tax.");
        }

        [Test, Category("Regression")]
        public void SubtotalMatchesSumOfItemPrices()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack, TestData.Products.Onesie);

            decimal sumOfPrices = Pages.CheckoutStepTwo.GetItemPrices()
                .Select(p => decimal.Parse(p.TrimStart('$')))
                .Sum();

            Assert.That(Pages.CheckoutStepTwo.GetSubtotalAmount(), Is.EqualTo(sumOfPrices),
                "Subtotal should equal the sum of individual item prices.");
        }

        [Test, Category("Smoke")]
        public void FinishNavigatesToCompletePage()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Pages.CheckoutStepTwo.Finish();
            Pages.CheckoutComplete.WaitToLoad();

            Assert.That(Pages.CheckoutComplete.GetHeaderText(), Is.EqualTo("Checkout: Complete!"),
                "Finish should navigate to the checkout complete page.");
        }

        [Test, Category("Regression")]
        public void CheckoutStepTwoCancelReturnsToInventory()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Pages.CheckoutStepTwo.Cancel();
            Pages.Inventory.WaitToLoad();

            Assert.That(Pages.Inventory.GetHeaderText(), Is.EqualTo("Products"),
                "Cancel should navigate back to the inventory page.");
        }

        // ── Complete ─────────────────────────────────────────────────

        [Test, Category("Smoke")]
        public void CheckoutCompleteDisplaysCorrectHeader()
        {
            NavigateToCheckoutComplete(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutComplete.GetHeaderText(), Is.EqualTo("Checkout: Complete!"),
                "Checkout complete header should be 'Checkout: Complete!'.");
        }

        [Test, Category("Smoke")]
        public void ThankYouMessageIsDisplayed()
        {
            NavigateToCheckoutComplete(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"),
                "Thank you message should be displayed.");
        }

        [Test, Category("Regression")]
        public void ConfirmationTextIsDisplayed()
        {
            NavigateToCheckoutComplete(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutComplete.GetCompleteText(), Is.Not.Empty,
                "Confirmation body text should be displayed.");
            Assert.That(Pages.CheckoutComplete.GetCompleteText(), Does.Contain("dispatched"),
                "Confirmation text should mention that the order has been dispatched.");
        }

        [Test, Category("Regression")]
        public void PonyExpressImageIsDisplayed()
        {
            NavigateToCheckoutComplete(TestData.Products.Backpack);

            Assert.That(Pages.CheckoutComplete.IsPonyExpressImageDisplayed(), Is.True,
                "Pony express image should be displayed on the complete page.");
        }

        [Test, Category("Smoke")]
        public void BackHomeReturnsToInventory()
        {
            NavigateToCheckoutComplete(TestData.Products.Backpack);

            Pages.CheckoutComplete.BackHome();
            Pages.Inventory.WaitToLoad();

            Assert.That(Pages.Inventory.GetHeaderText(), Is.EqualTo("Products"),
                "Back Home should navigate to the inventory page.");
        }
    }
}