using NUnit.Framework;
using SeleniumTestbase;
using Assert = NUnit.Framework.Assert;

namespace UiTests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Children)]
    public class CartTests : TestBase
    {
        [Test, Category("Smoke")]
        public void CartPageDisplaysCorrectHeader()
        {
            LoginAsStandard();

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.GetHeaderText(), Is.EqualTo("Your Cart"),
                "Cart page header should be 'Your Cart'.");
        }

        [Test, Category("Regression")]
        public void EmptyCartShowsNoItems()
        {
            LoginAsStandard();

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.IsCartEmpty(), Is.True,
                "Cart should be empty when no items have been added.");
        }

        [Test, Category("Smoke")]
        public void AddedItemAppearsInCart()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.GetCartItemCount(), Is.EqualTo(1),
                "Cart should contain 1 item.");
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.Backpack), Is.True,
                $"{TestData.Products.Backpack} should be present in the cart.");
        }

        [Test, Category("Regression")]
        public void MultipleAddedItemsAppearInCart()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.FleeceJacket);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.GetCartItemCount(), Is.EqualTo(2),
                "Cart should contain 2 items.");
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.Backpack), Is.True,
                $"{TestData.Products.Backpack} should be in the cart.");
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.FleeceJacket), Is.True,
                $"{TestData.Products.FleeceJacket} should be in the cart.");
        }

        [Test, Category("Regression")]
        public void CartItemsDisplayCorrectPrices()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.Onesie);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            List<string> prices = Pages.Cart.GetItemPrices();

            Assert.That(prices, Has.Count.EqualTo(2),
                "Cart should show prices for both items.");
            Assert.That(prices, Has.All.StartsWith("$"),
                "All cart prices should start with a dollar sign.");
        }

        [Test, Category("Regression")]
        public void CartItemQuantitiesAreOne()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.BoltTShirt);
            Pages.Inventory.AddItemToCartByName(TestData.Products.BikeLight);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.GetItemQuantities(), Has.All.EqualTo(1),
                "Each cart item quantity should be 1.");
        }

        [Test, Category("Smoke")]
        public void RemoveItemFromCart()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.BikeLight);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Pages.Cart.RemoveItemByName(TestData.Products.Backpack);

            Assert.That(Pages.Cart.GetCartItemCount(), Is.EqualTo(1),
                "Cart should contain 1 item after removal.");
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.Backpack), Is.False,
                "Removed item should no longer appear in the cart.");
            Assert.That(Pages.Cart.IsItemInCart(TestData.Products.BikeLight), Is.True,
                "Remaining item should still be in the cart.");
        }

        [Test, Category("Regression")]
        public void RemoveAllItemsFromCart()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.BikeLight);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Pages.Cart.RemoveItemByName(TestData.Products.Backpack);
            Pages.Cart.RemoveItemByName(TestData.Products.BikeLight);

            Assert.That(Pages.Cart.IsCartEmpty(), Is.True,
                "Cart should be empty after removing all items.");
            Assert.That(Pages.Cart.GetCartBadgeCount(), Is.EqualTo(0),
                "Cart badge should not be visible when cart is empty.");
        }

        [Test, Category("Regression")]
        public void ContinueShoppingReturnsToInventory()
        {
            LoginAsStandard();

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Pages.Cart.ContinueShopping();
            Pages.Inventory.WaitToLoad();

            Assert.That(Pages.Inventory.GetHeaderText(), Is.EqualTo("Products"),
                "Should navigate back to the inventory page.");
        }

        [Test, Category("Smoke")]
        public void CheckoutNavigatesToCheckoutPage()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Pages.Cart.Checkout();

            Assert.That(Session?.CurrentUrl, Does.Contain("checkout-step-one"),
                "Should navigate to the checkout step one page.");
        }

        [Test, Category("Regression")]
        public void CartBadgePersistsAcrossPages()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.BoltTShirt);

            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(2),
                "Cart badge should show 2 on inventory page.");

            Pages.Inventory.GoToCart();
            Pages.Cart.WaitToLoad();

            Assert.That(Pages.Cart.GetCartBadgeCount(), Is.EqualTo(2),
                "Cart badge should still show 2 on the cart page.");

            Pages.Cart.ContinueShopping();
            Pages.Inventory.WaitToLoad();

            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(2),
                "Cart badge should still show 2 after returning to inventory.");
        }
    }
}