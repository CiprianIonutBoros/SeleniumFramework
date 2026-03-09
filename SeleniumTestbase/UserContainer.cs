    namespace SeleniumTestbase;

/// <summary>
/// Root deserialization target for users.json.
/// Contains the collection of test user credentials.
/// </summary>
public class UserContainer
{
    /// <summary>
    /// List of all configured test users loaded from users.json.
    /// </summary>
    public List<User>? Users { get; set; } = new List<User>();
}

/// <summary>
/// Represents a single test user with login credentials
/// and a <see cref="UserType"/> classification.
/// </summary>
public class User
{
    /// <summary>
    /// The username used for login (e.g. "standard_user").
    /// </summary>
    public string? Username { get; set; } = "";

    /// <summary>
    /// The password used for login.
    /// </summary>
    public string? Password { get; set; } = "";

    /// <summary>
    /// The user type classification that determines test behavior.
    /// </summary>
    public UserType Type { get; set; }
}

/// <summary>
/// Enumerates the different user personas available on the SauceDemo site.
/// Each type exercises different application behavior.
/// </summary>
public enum UserType
{
    /// <summary>Normal user with full access.</summary>
    Standard,
    /// <summary>User that is locked out and cannot log in.</summary>
    Locked,
    /// <summary>User that triggers problematic UI behavior.</summary>
    Problematic,
    /// <summary>User that experiences random delays (performance glitches).</summary>
    Glitch,
    /// <summary>User that triggers server-side errors on certain actions.</summary>
    Error,
    /// <summary>User that triggers visual/rendering bugs.</summary>
    Visual
}