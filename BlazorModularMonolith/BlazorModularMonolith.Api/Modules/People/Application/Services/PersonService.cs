using BlazorModularMonolith.Api.Modules.People.Application.DTOs;
using BlazorModularMonolith.Api.Modules.People.Domain.Entities;
using BlazorModularMonolith.Api.Modules.People.Domain.Repositories;
using BlazorModularMonolith.Api.Shared.Common;

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

    public async Task<Result<PersonDto>> GetPersonAsync(Guid id)
    {
        _logger.LogInformation("Retrieving person with ID: {PersonId}", id);
        var person = await _repository.GetByIdAsync(id);
        return person is not null
            ? Result<PersonDto>.Success(MapToDto(person))
            : Result<PersonDto>.Failure($"Person with ID '{id}' not found.");
    }

    public async Task<Result<IEnumerable<PersonDto>>> GetAllPeopleAsync()
    {
        _logger.LogInformation("Retrieving all people");
        var people = await _repository.GetAllAsync();
        return Result<IEnumerable<PersonDto>>.Success(people.Select(MapToDto));
    }

    public async Task<Result<PersonDto>> GetPersonByEmailAsync(string email)
    {
        _logger.LogInformation("Retrieving person with email: {Email}", email);
        var person = await _repository.GetByEmailAsync(email);
        return person is not null
            ? Result<PersonDto>.Success(MapToDto(person))
            : Result<PersonDto>.Failure($"Person with email '{email}' not found.");
    }

    public async Task<Result<PersonDto>> CreatePersonAsync(CreatePersonRequest request)
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
        return Result<PersonDto>.Success(MapToDto(created));
    }

    public async Task<Result<PersonDto>> UpdatePersonAsync(Guid id, UpdatePersonRequest request)
    {
        _logger.LogInformation("Updating person with ID: {PersonId}", id);

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found", id);
            return Result<PersonDto>.Failure($"Person with ID '{id}' not found.");
        }

        existing.FirstName = request.FirstName;
        existing.LastName = request.LastName;
        existing.Email = request.Email;
        existing.PhoneNumber = request.PhoneNumber;
        existing.DateOfBirth = request.DateOfBirth;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        return updated is not null
            ? Result<PersonDto>.Success(MapToDto(updated))
            : Result<PersonDto>.Failure($"Person with ID '{id}' not found.");
    }

    public async Task<Result> DeletePersonAsync(Guid id)
    {
        _logger.LogInformation("Deleting person with ID: {PersonId}", id);
        var deleted = await _repository.DeleteAsync(id);
        return deleted
            ? Result.Success()
            : Result.Failure($"Person with ID '{id}' not found.");
    }

    public async Task<Result<PersonDto>> AddAddressToPersonAsync(Guid personId, Guid addressId)
    {
        _logger.LogInformation("Adding address {AddressId} to person {PersonId}", addressId, personId);

        var person = await _repository.GetByIdAsync(personId);
        if (person == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found", personId);
            return Result<PersonDto>.Failure($"Person with ID '{personId}' not found.");
        }

        if (!person.AddressIds.Contains(addressId))
        {
            person.AddressIds.Add(addressId);
            person.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(person);
        }

        return Result<PersonDto>.Success(MapToDto(person));
    }

    public async Task<Result<PersonDto>> RemoveAddressFromPersonAsync(Guid personId, Guid addressId)
    {
        _logger.LogInformation("Removing address {AddressId} from person {PersonId}", addressId, personId);

        var person = await _repository.GetByIdAsync(personId);
        if (person == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found", personId);
            return Result<PersonDto>.Failure($"Person with ID '{personId}' not found.");
        }

        person.AddressIds.Remove(addressId);
        person.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(person);

        return Result<PersonDto>.Success(MapToDto(person));
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
