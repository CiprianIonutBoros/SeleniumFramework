using NUnit.Framework;
using SeleniumTestbase;
using Assert = NUnit.Framework.Assert;

namespace UiTests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Children)]
    public class EndToEndTests : TestBase
    {
        [Test, Category("Smoke")]
        public void CompletePurchaseSingleItem()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(1));

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.Backpack), Is.True);

            Pages.Cart.Checkout();
            Pages.CheckoutStepOne.WaitToLoad();
            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Pages.CheckoutStepTwo.WaitToLoad();
            Assert.That(Pages.CheckoutStepTwo.IsItemInSummary(TestData.Products.Backpack), Is.True);
            Assert.That(Pages.CheckoutStepTwo.GetTotalAmount(), Is.GreaterThan(0));
            Pages.CheckoutStepTwo.Finish();

            Pages.CheckoutComplete.WaitToLoad();
            Assert.That(Pages.CheckoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"));

            Pages.CheckoutComplete.BackHome();
            Pages.Inventory.WaitToLoad();
            Assert.That(Pages.Inventory.GetHeaderText(), Is.EqualTo("Products"));
        }

        [Test, Category("Smoke")]
        public void CompletePurchaseMultipleItems()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.BikeLight);
            Pages.Inventory.AddItemToCartByName(TestData.Products.FleeceJacket);
            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(3));

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Assert.That(Pages.Cart.GetCartItemCount(), Is.EqualTo(3));

            Pages.Cart.Checkout();
            Pages.CheckoutStepOne.WaitToLoad();
            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Pages.CheckoutStepTwo.WaitToLoad();
            Assert.That(Pages.CheckoutStepTwo.GetItemCount(), Is.EqualTo(3));

            decimal subtotal = Pages.CheckoutStepTwo.GetSubtotalAmount();
            decimal tax = Pages.CheckoutStepTwo.GetTaxAmount();
            decimal total = Pages.CheckoutStepTwo.GetTotalAmount();
            Assert.That(total, Is.EqualTo(subtotal + tax).Within(0.01m));

            Pages.CheckoutStepTwo.Finish();

            Pages.CheckoutComplete.WaitToLoad();
            Assert.That(Pages.CheckoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"));
        }

        [Test, Category("Regression")]
        public void RemoveItemThenCompletePurchase()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.Onesie);

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Pages.Cart.RemoveItemByName(TestData.Products.Onesie);
            Assert.That(Pages.Cart.GetCartItemCount(), Is.EqualTo(1));
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.Onesie), Is.False);

            Pages.Cart.Checkout();
            Pages.CheckoutStepOne.WaitToLoad();
            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Pages.CheckoutStepTwo.WaitToLoad();
            Assert.That(Pages.CheckoutStepTwo.GetItemCount(), Is.EqualTo(1));
            Pages.CheckoutStepTwo.Finish();

            Pages.CheckoutComplete.WaitToLoad();
            Assert.That(Pages.CheckoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"));
        }

        [Test, Category("Regression")]
        public void CancelAtStepOneReturnsToCart()
        {
            NavigateToCheckoutStepOne(TestData.Products.Backpack);

            Pages.CheckoutStepOne.Cancel();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.GetHeaderText(), Is.EqualTo("Your Cart"));
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.Backpack), Is.True);
        }

        [Test, Category("Regression")]
        public void CancelAtStepTwoReturnsToInventory()
        {
            NavigateToCheckoutStepTwo(TestData.Products.Backpack);

            Pages.CheckoutStepTwo.Cancel();
            Pages.Inventory.WaitToLoad();

            Assert.That(Pages.Inventory.GetHeaderText(), Is.EqualTo("Products"));
            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(1));
        }

        [Test, Category("Regression")]
        public void ContinueShoppingThenCompletePurchase()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Pages.Cart.ContinueShopping();
            Pages.Inventory.WaitToLoad();

            Pages.Inventory.AddItemToCartByName(TestData.Products.BoltTShirt);
            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(2));

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Pages.Cart.Checkout();

            Pages.CheckoutStepOne.WaitToLoad();
            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Pages.CheckoutStepTwo.WaitToLoad();
            Assert.That(Pages.CheckoutStepTwo.GetItemCount(), Is.EqualTo(2));
            Pages.CheckoutStepTwo.Finish();

            Pages.CheckoutComplete.WaitToLoad();
            Assert.That(Pages.CheckoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"));
        }

        [Test, Category("Regression")]
        public void SortByPriceThenPurchaseCheapestItem()
        {
            LoginAsStandard();

            Pages.Inventory.SortBy("lohi");
            string cheapest = Pages.Inventory.GetItemNames().First();
            Pages.Inventory.AddItemToCartByName(cheapest);

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Assert.That(Pages.Cart.IsItemInCart(cheapest), Is.True);
            Pages.Cart.Checkout();

            Pages.CheckoutStepOne.WaitToLoad();
            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Pages.CheckoutStepTwo.WaitToLoad();
            Assert.That(Pages.CheckoutStepTwo.IsItemInSummary(cheapest), Is.True);
            Pages.CheckoutStepTwo.Finish();

            Pages.CheckoutComplete.WaitToLoad();
            Assert.That(Pages.CheckoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"));
        }

        [Test, Category("Regression")]
        public void PurchaseAllSixItems()
        {
            LoginAsStandard();

            foreach (string product in TestData.Products.All)
                Pages.Inventory.AddItemToCartByName(product);

            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(6));

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();
            Assert.That(Pages.Cart.GetCartItemCount(), Is.EqualTo(6));

            Pages.Cart.Checkout();
            Pages.CheckoutStepOne.WaitToLoad();
            Pages.CheckoutStepOne.SubmitForm(
                TestData.Checkout.FirstName,
                TestData.Checkout.LastName,
                TestData.Checkout.PostalCode);

            Pages.CheckoutStepTwo.WaitToLoad();
            Assert.That(Pages.CheckoutStepTwo.GetItemCount(), Is.EqualTo(6));
            foreach (string product in TestData.Products.All)
                Assert.That(Pages.CheckoutStepTwo.IsItemInSummary(product), Is.True, $"{product} should be in overview.");

            decimal subtotal = Pages.CheckoutStepTwo.GetSubtotalAmount();
            decimal tax = Pages.CheckoutStepTwo.GetTaxAmount();
            decimal total = Pages.CheckoutStepTwo.GetTotalAmount();
            decimal sumOfPrices = Pages.CheckoutStepTwo.GetItemPrices()
                .Select(p => decimal.Parse(p.TrimStart('$')))
                .Sum();

            Assert.That(subtotal, Is.EqualTo(sumOfPrices));
            Assert.That(total, Is.EqualTo(subtotal + tax).Within(0.01m));

            Pages.CheckoutStepTwo.Finish();

            Pages.CheckoutComplete.WaitToLoad();
            Assert.That(Pages.CheckoutComplete.GetCompleteHeaderText(), Is.EqualTo("Thank you for your order!"));
            Pages.CheckoutComplete.BackHome();
            Pages.Inventory.WaitToLoad();
            Assert.That(Pages.Inventory.GetHeaderText(), Is.EqualTo("Products"));
        }

        [Test, Category("Regression")]
        public void LogoutAfterCompletingPurchase()
        {
            NavigateToCheckoutComplete(TestData.Products.Backpack);

            Pages.CheckoutComplete.BackHome();
            Pages.Inventory.WaitToLoad();
            Pages.Inventory.Logout();
            Pages.Login.WaitToLoad();

            Assert.That(Session?.CurrentUrl, Does.Contain("saucedemo.com"),
                "User should be on the login page after logout.");
        }
    }
}