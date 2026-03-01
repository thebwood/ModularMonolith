using BlazorModularMonolith.Api.Modules.People.Application.DTOs;
using BlazorModularMonolith.Api.Modules.People.Domain.Entities;
using BlazorModularMonolith.Api.Modules.People.Domain.Repositories;

namespace BlazorModularMonolith.Api.Modules.People.Application.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;
    private readonly ILogger<PersonService> _logger;

    public PersonService(IPersonRepository repository, ILogger<PersonService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PersonDto?> GetPersonAsync(Guid id)
    {
        _logger.LogInformation("Retrieving person with ID: {PersonId}", id);
        var person = await _repository.GetByIdAsync(id);
        return person != null ? MapToDto(person) : null;
    }

    public async Task<IEnumerable<PersonDto>> GetAllPeopleAsync()
    {
        _logger.LogInformation("Retrieving all people");
        var people = await _repository.GetAllAsync();
        return people.Select(MapToDto);
    }

    public async Task<PersonDto?> GetPersonByEmailAsync(string email)
    {
        _logger.LogInformation("Retrieving person with email: {Email}", email);
        var person = await _repository.GetByEmailAsync(email);
        return person != null ? MapToDto(person) : null;
    }

    public async Task<PersonDto> CreatePersonAsync(CreatePersonRequest request)
    {
        _logger.LogInformation("Creating new person: {FirstName} {LastName}", request.FirstName, request.LastName);
        
        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(person);
        return MapToDto(created);
    }

    public async Task<PersonDto?> UpdatePersonAsync(Guid id, UpdatePersonRequest request)
    {
        _logger.LogInformation("Updating person with ID: {PersonId}", id);
        
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found", id);
            return null;
        }

        existing.FirstName = request.FirstName;
        existing.LastName = request.LastName;
        existing.Email = request.Email;
        existing.PhoneNumber = request.PhoneNumber;
        existing.DateOfBirth = request.DateOfBirth;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        return updated != null ? MapToDto(updated) : null;
    }

    public async Task<bool> DeletePersonAsync(Guid id)
    {
        _logger.LogInformation("Deleting person with ID: {PersonId}", id);
        return await _repository.DeleteAsync(id);
    }

    public async Task<PersonDto?> AddAddressToPersonAsync(Guid personId, Guid addressId)
    {
        _logger.LogInformation("Adding address {AddressId} to person {PersonId}", addressId, personId);
        
        var person = await _repository.GetByIdAsync(personId);
        if (person == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found", personId);
            return null;
        }

        if (!person.AddressIds.Contains(addressId))
        {
            person.AddressIds.Add(addressId);
            person.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(person);
        }

        return MapToDto(person);
    }

    public async Task<PersonDto?> RemoveAddressFromPersonAsync(Guid personId, Guid addressId)
    {
        _logger.LogInformation("Removing address {AddressId} from person {PersonId}", addressId, personId);
        
        var person = await _repository.GetByIdAsync(personId);
        if (person == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found", personId);
            return null;
        }

        person.AddressIds.Remove(addressId);
        person.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(person);

        return MapToDto(person);
    }

    private static PersonDto MapToDto(Person person) => new(
        person.Id,
        person.FirstName,
        person.LastName,
        person.Email,
        person.PhoneNumber,
        person.DateOfBirth,
        person.AddressIds,
        person.CreatedAt,
        person.UpdatedAt
    );
}
