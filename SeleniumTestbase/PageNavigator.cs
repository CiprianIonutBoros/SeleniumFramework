using OpenQA.Selenium;

namespace SeleniumTestbase
{
    /// <summary>
    /// Provides lazy-initialized, cached page object instances tied to the
    /// current <see cref="BrowserSession"/>. Eliminates repetitive page
    /// construction in tests and centralizes the page registry.
    /// <para>
    /// <b>To add a new page:</b>
    /// <list type="number">
    ///   <item>Create <c>PageMaps\NewPage.cs</c> — locators (partial class)</item>
    ///   <item>Create <c>Pages\NewPage.cs</c> — behavior (partial class : BasePage)</item>
    ///   <item>Add a property here: <c>public NewPage NewPage => Get(() => new NewPage(_driver, _session));</c></item>
    /// </list>
    /// </para>
    /// </summary>
    public class PageNavigator
    {
        private readonly IWebDriver? _driver;
        private readonly BrowserSession? _session;

        /// <summary>
        /// Internal cache keyed by page type. Prevents re-creating page objects
        /// within the same test — each page is instantiated at most once.
        /// </summary>
        private readonly Dictionary<Type, BasePage> _cache = new();

        /// <summary>
        /// Initializes the navigator with the given browser session.
        /// </summary>
        /// <param name="session">The active browser session for this test.</param>
        public PageNavigator(BrowserSession? session)
        {
            _session = session;
            _driver = session?.Driver;
        }

        /// <summary>Login page — username, password, submit.</summary>
        public LoginPage Login => Get(() => new LoginPage(_driver, _session));

        /// <summary>Inventory/products page — item listing, sort, cart actions.</summary>
        public InventoryPage Inventory => Get(() => new InventoryPage(_driver, _session));

        /// <summary>Shopping cart page — item review, remove, continue/checkout.</summary>
        public CartPage Cart => Get(() => new CartPage(_driver, _session));

        /// <summary>Checkout step one — customer information form.</summary>
        public CheckoutStepOnePage CheckoutStepOne => Get(() => new CheckoutStepOnePage(_driver, _session));

        /// <summary>Checkout step two — order overview with pricing summary.</summary>
        public CheckoutStepTwoPage CheckoutStepTwo => Get(() => new CheckoutStepTwoPage(_driver, _session));

        /// <summary>Checkout complete — order confirmation and back-home navigation.</summary>
        public CheckoutCompletePage CheckoutComplete => Get(() => new CheckoutCompletePage(_driver, _session));

        /// <summary>
        /// Returns a cached page instance, or creates and caches one using the factory.
        /// </summary>
        /// <typeparam name="T">The concrete page type.</typeparam>
        /// <param name="factory">A factory delegate that creates the page instance.</param>
        /// <returns>The cached or newly created page instance.</returns>
        private T Get<T>(Func<T> factory) where T : BasePage
        {
            if (!_cache.TryGetValue(typeof(T), out BasePage? page))
            {
                page = factory();
                _cache[typeof(T)] = page;
            }
            return (T)page;
        }
    }
}