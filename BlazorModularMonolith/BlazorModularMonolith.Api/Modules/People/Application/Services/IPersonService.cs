using BlazorModularMonolith.Api.Modules.People.Application.DTOs;

namespace BlazorModularMonolith.Api.Modules.People.Application.Services;

public interface IPersonService
{
    Task<PersonDto?> GetPersonAsync(Guid id);
    Task<IEnumerable<PersonDto>> GetAllPeopleAsync();
    Task<PersonDto?> GetPersonByEmailAsync(string email);
    Task<PersonDto> CreatePersonAsync(CreatePersonRequest request);
    Task<PersonDto?> UpdatePersonAsync(Guid id, UpdatePersonRequest request);
    Task<bool> DeletePersonAsync(Guid id);
    Task<PersonDto?> AddAddressToPersonAsync(Guid personId, Guid addressId);
    Task<PersonDto?> RemoveAddressFromPersonAsync(Guid personId, Guid addressId);
}
