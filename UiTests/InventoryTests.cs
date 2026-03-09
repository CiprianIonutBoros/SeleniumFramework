using NUnit.Framework;
using SeleniumTestbase;
using Assert = NUnit.Framework.Assert;

namespace UiTests
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Children)]
    public class InventoryTests : TestBase
    {
        [Test, Category("Smoke")]
        public void InventoryPageDisplaysProducts()
        {
            LoginAsStandard();

            Assert.That(Pages.Inventory.GetHeaderText(), Is.EqualTo("Products"),
                "Inventory page header should be 'Products'.");
            Assert.That(Pages.Inventory.GetItemCount(), Is.EqualTo(6),
                "Inventory page should display 6 products.");
        }

        [Test, Category("Regression")]
        public void InventoryPageDisplaysAllItemNames()
        {
            LoginAsStandard();

            List<string> itemNames = Pages.Inventory.GetItemNames();

            Assert.That(itemNames, Has.Count.EqualTo(TestData.Products.All.Length),
                $"Should display {TestData.Products.All.Length} item names.");
            foreach (string product in TestData.Products.All)
                Assert.That(itemNames, Does.Contain(product), $"'{product}' should be displayed.");
        }

        [Test, Category("Regression")]
        public void InventoryItemsHaveValidPrices()
        {
            LoginAsStandard();

            List<string> prices = Pages.Inventory.GetItemPrices();

            Assert.That(prices, Has.Count.EqualTo(TestData.Products.All.Length),
                "All items should have a price displayed.");
            Assert.That(prices, Has.All.StartsWith("$"),
                "All prices should start with a dollar sign.");
        }

        [Test, Category("Regression")]
        public void InventoryItemImagesAreValid()
        {
            LoginAsStandard();

            Assert.That(Pages.Inventory.AllItemImagesAreValid(), Is.True,
                "All inventory item images should have a valid src attribute.");
        }

        [Test, Category("Smoke")]
        public void AddSingleItemToCartUpdatesCartBadge()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);

            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(1),
                "Cart badge should show 1 after adding a single item.");
        }

        [Test, Category("Regression")]
        public void AddMultipleItemsToCartUpdatesCartBadge()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.BikeLight);
            Pages.Inventory.AddItemToCartByName(TestData.Products.Onesie);

            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(3),
                "Cart badge should show 3 after adding three items.");
        }

        [Test, Category("Regression")]
        public void RemoveItemFromCartOnInventoryPage()
        {
            LoginAsStandard();

            Pages.Inventory.AddItemToCartByName(TestData.Products.Backpack);
            Pages.Inventory.AddItemToCartByName(TestData.Products.BikeLight);

            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(2),
                "Cart badge should show 2 after adding two items.");

            Pages.Inventory.RemoveItemByName(TestData.Products.Backpack);

            Assert.That(Pages.Inventory.GetCartBadgeCount(), Is.EqualTo(1),
                "Cart badge should show 1 after removing one item.");
        }

        [Test, Category("Regression")]
        public void SortItemsByPriceLowToHigh()
        {
            LoginAsStandard();

            Pages.Inventory.SortBy("lohi");

            List<decimal> parsedPrices = Pages.Inventory.GetItemPrices()
                .Select(p => decimal.Parse(p.TrimStart('$')))
                .ToList();

            Assert.That(parsedPrices, Is.Ordered.Ascending,
                "Items should be sorted by price from low to high.");
        }

        [Test, Category("Regression")]
        public void SortItemsByPriceHighToLow()
        {
            LoginAsStandard();

            Pages.Inventory.SortBy("hilo");

            List<decimal> parsedPrices = Pages.Inventory.GetItemPrices()
                .Select(p => decimal.Parse(p.TrimStart('$')))
                .ToList();

            Assert.That(parsedPrices, Is.Ordered.Descending,
                "Items should be sorted by price from high to low.");
        }

        [Test, Category("Regression")]
        public void SortItemsByNameAToZ()
        {
            LoginAsStandard();

            Pages.Inventory.SortBy("az");

            Assert.That(Pages.Inventory.GetItemNames(), Is.Ordered.Ascending,
                "Items should be sorted alphabetically A to Z.");
        }

        [Test, Category("Regression")]
        public void SortItemsByNameZToA()
        {
            LoginAsStandard();

            Pages.Inventory.SortBy("za");

            Assert.That(Pages.Inventory.GetItemNames(), Is.Ordered.Descending,
                "Items should be sorted alphabetically Z to A.");
        }

        [Test, Category("Smoke")]
        public void LogoutFromInventoryPage()
        {
            LoginAsStandard();

            Pages.Inventory.Logout();
            Pages.Login.WaitToLoad();

            Assert.That(Session?.CurrentUrl, Does.Contain("saucedemo.com"),
                "User should be redirected back to the login page after logout.");
        }
    }
}