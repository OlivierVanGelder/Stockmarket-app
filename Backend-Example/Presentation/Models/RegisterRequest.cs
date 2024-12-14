namespace Backend_Example.Models;

    // Used for deserialization
    public class RegisterRequest
    {
        public string Name { get; } = string.Empty;
        public string Password { get; } = string.Empty;
    }
