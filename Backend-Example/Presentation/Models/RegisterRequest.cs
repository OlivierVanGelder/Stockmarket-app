namespace Backend_Example.Models;

// Used for deserialization
public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
