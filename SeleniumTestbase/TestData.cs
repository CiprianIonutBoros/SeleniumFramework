namespace SeleniumTestbase
{
    /// <summary>
    /// Centralized test data constants used across all test fixtures.
    /// Keeps magic strings out of test methods and makes bulk updates trivial.
    /// </summary>
    public static class TestData
    {
        /// <summary>
        /// Product names as displayed on the SauceDemo inventory page.
        /// </summary>
        public static class Products
        {
            public const string Backpack = "Sauce Labs Backpack";
            public const string BikeLight = "Sauce Labs Bike Light";
            public const string BoltTShirt = "Sauce Labs Bolt T-Shirt";
            public const string FleeceJacket = "Sauce Labs Fleece Jacket";
            public const string Onesie = "Sauce Labs Onesie";
            public const string RedTShirt = "Test.allTheThings() T-Shirt (Red)";

            /// <summary>All product names in catalog order for iteration.</summary>
            public static readonly string[] All =
            [
                Backpack, BikeLight, BoltTShirt, FleeceJacket, Onesie, RedTShirt
            ];
        }

        /// <summary>
        /// Expected price strings including the dollar sign, matching the site's display format.
        /// </summary>
        public static class ExpectedPrices
        {
            public const string Backpack = "$29.99";
            public const string BikeLight = "$9.99";
            public const string BoltTShirt = "$15.99";
            public const string FleeceJacket = "$49.99";
            public const string Onesie = "$7.99";
            public const string RedTShirt = "$15.99";
        }

        /// <summary>
        /// Customer information used to fill the checkout step one form.
        /// </summary>
        public static class Checkout
        {
            public const string FirstName = "John";
            public const string LastName = "Doe";
            public const string PostalCode = "12345";
        }
    }
}