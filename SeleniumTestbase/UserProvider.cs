using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SeleniumTestbase
{
    /// <summary>
    /// Provides test user credentials by deserializing Users.json on first access.
    /// Uses lazy initialization so the file is read only once per test run,
    /// and the result is shared across all parallel test instances.
    /// </summary>
    public static class UserProvider
    {
        /// <summary>
        /// Newtonsoft settings with string-based enum conversion so
        /// "Standard" in JSON maps to <see cref="UserType.Standard"/>.
        /// </summary>
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            Converters = { new StringEnumConverter() }
        };

        /// <summary>
        /// Thread-safe lazy container that reads and deserializes Users.json once.
        /// </summary>
        private static readonly Lazy<UserContainer> Container = new(() =>
        {
            string json = File.ReadAllText("Users.json");

            return JsonConvert.DeserializeObject<UserContainer>(json, JsonSettings)
                   ?? throw new InvalidOperationException("Failed to deserialize Users.json.");
        });

        /// <summary>
        /// Returns the <see cref="User"/> matching the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The user persona to retrieve.</param>
        /// <returns>The matching <see cref="User"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when no user matches the given type.</exception>
        public static User GetByType(UserType type)
        {
            User? user = Container.Value.Users?.FirstOrDefault(u => u.Type == type);
            if (user == null)
            {
                string available = string.Join(", ", Container.Value.Users?.Select(u => u.Type.ToString()) ?? []);
                throw new ArgumentException($"User type '{type}' not found. Available: {available}");
            }
            return user;
        }
    }
}