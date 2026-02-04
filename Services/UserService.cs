using Microsoft.AspNetCore.Identity;
using PasswordAdmin.Models;

namespace PasswordAdmin.Services;

public class UserService
{
    private readonly PasswordHasher<User> _hasher = new();

    // En memoria por ahora (después lo pasamos a DB si el equipo quiere)
    private readonly List<User> _users = new();

    public (bool ok, string message) Register(string email, string password)
    {
        email = (email ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.");

        if (_users.Any(u => u.Email == email))
            return (false, "That email is already registered.");

        var user = new User { Email = email };
        user.PasswordHash = _hasher.HashPassword(user, password);

        _users.Add(user);
        return (true, "Account created successfully.");
    }

    public (bool ok, string message, User? user) Login(string email, string password)
    {
        email = (email ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (false, "Email and password are required.", null);

        var user = _users.FirstOrDefault(u => u.Email == email);
        if (user == null)
            return (false, "Invalid email or password.", null);

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return (false, "Invalid email or password.", null);

        return (true, "Login successful.", user);
    }

    public User? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);
}
