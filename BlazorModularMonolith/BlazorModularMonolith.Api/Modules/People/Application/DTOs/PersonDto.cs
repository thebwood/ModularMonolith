namespace BlazorModularMonolith.Api.Modules.People.Application.DTOs;

public record PersonDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    List<Guid> AddressIds,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreatePersonRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth
);

public record UpdatePersonRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth
);

public record AddAddressToPersonRequest(
    Guid AddressId
);
