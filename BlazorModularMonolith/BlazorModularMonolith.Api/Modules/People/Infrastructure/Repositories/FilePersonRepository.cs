using System.Text.Json;
using BlazorModularMonolith.Api.Modules.People.Domain.Entities;
using BlazorModularMonolith.Api.Modules.People.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.People.Infrastructure.Repositories;

public class FilePersonRepository : IPersonRepository
{
    private readonly string _filePath;
    private readonly ILogger<FilePersonRepository> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public FilePersonRepository(IConfiguration configuration, ILogger<FilePersonRepository> logger)
    {
        _logger = logger;
        var dataDirectory = configuration["Storage:DataDirectory"] ?? "Data";
        
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
        
        _filePath = Path.Combine(dataDirectory, "people.json");
        
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<Person?> GetByIdAsync(Guid id)
    {
        var people = await ReadAllAsync();
        return people.FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        return await ReadAllAsync();
    }

    public async Task<Person?> GetByEmailAsync(string email)
    {
        var people = await ReadAllAsync();
        return people.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Person> CreateAsync(Person person)
    {
        await _semaphore.WaitAsync();
        try
        {
            var people = (await ReadAllAsync()).ToList();
            people.Add(person);
            await WriteAllAsync(people);
            _logger.LogInformation("Created person with ID: {PersonId}", person.Id);
            return person;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Person?> UpdateAsync(Person person)
    {
        await _semaphore.WaitAsync();
        try
        {
            var people = (await ReadAllAsync()).ToList();
            var index = people.FindIndex(p => p.Id == person.Id);
            
            if (index == -1)
            {
                return null;
            }

            people[index] = person;
            await WriteAllAsync(people);
            _logger.LogInformation("Updated person with ID: {PersonId}", person.Id);
            return person;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var people = (await ReadAllAsync()).ToList();
            var removed = people.RemoveAll(p => p.Id == id);
            
            if (removed == 0)
            {
                return false;
            }

            await WriteAllAsync(people);
            _logger.LogInformation("Deleted person with ID: {PersonId}", id);
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<Person>> ReadAllAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Person>>(json) ?? new List<Person>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading people from file");
            return new List<Person>();
        }
    }

    private async Task WriteAllAsync(List<Person> people)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(people, options);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing people to file");
            throw;
        }
    }
}
