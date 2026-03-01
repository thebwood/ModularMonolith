namespace BlazorModularMonolith.Api.Shared.Contracts;

public record AddressCreatedEvent(
    Guid AddressId,
    Guid OwnerId,
    string OwnerName,
    string AddressType,
    DateTime CreatedAt
);

public record AddressDeletedEvent(
    Guid AddressId,
    Guid OwnerId,
    DateTime DeletedAt
);

public record PersonCreatedEvent(
    Guid PersonId,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt
);

public record BusinessCreatedEvent(
    Guid BusinessId,
    string Name,
    string TaxId,
    DateTime CreatedAt
);
