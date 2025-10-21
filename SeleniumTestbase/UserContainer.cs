namespace SeleniumTestbase;

public class UserContainer
{
    public List<User>? Users { get; set; } = new List<User>();
}

public class User
{
    public string? Username { get; set; } = "";

    public string? Password { get; set; } = "";

    public UserType Type { get; set; }
}
public enum UserType
{
    Standard,
    Locked,
    Problematic,
    Glitch,
    Error,
    Visual
}