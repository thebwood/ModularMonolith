using System.Text.Json;
using BlazorModularMonolith.Api.Modules.Authentication.Domain.Entities;
using BlazorModularMonolith.Api.Modules.Authentication.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.Authentication.Infrastructure.Repositories;

public class FileUserRepository : IUserRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileUserRepository(IConfiguration configuration)
    {
        var dataDirectory = configuration.GetValue<string>("Storage:DataDirectory") ?? "Data";
        _filePath = Path.Combine(dataDirectory, "users.json");
        
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        InitializeDefaultUsers();
    }

    private void InitializeDefaultUsers()
    {
        if (!File.Exists(_filePath))
        {
            var defaultUsers = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = HashPassword("Admin123!"),
                    Email = "admin@example.com",
                    Roles = new List<string> { "Admin", "User" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "user",
                    PasswordHash = HashPassword("User123!"),
                    Email = "user@example.com",
                    Roles = new List<string> { "User" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            var json = JsonSerializer.Serialize(defaultUsers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var users = await ReadUsersAsync();
        return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var users = await ReadUsersAsync();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User> CreateAsync(User user)
    {
        await _semaphore.WaitAsync();
        try
        {
            var users = await ReadUsersAsync();
            users.Add(user);
            await WriteUsersAsync(users);
            return user;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task UpdateAsync(User user)
    {
        await _semaphore.WaitAsync();
        try
        {
            var users = await ReadUsersAsync();
            var index = users.FindIndex(u => u.Id == user.Id);
            if (index != -1)
            {
                users[index] = user;
                await WriteUsersAsync(users);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await ReadUsersAsync();
    }

    private async Task<List<User>> ReadUsersAsync()
    {
        if (!File.Exists(_filePath))
            return new List<User>();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private async Task WriteUsersAsync(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}
