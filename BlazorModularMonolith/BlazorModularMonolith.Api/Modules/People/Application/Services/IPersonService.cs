using BlazorModularMonolith.Api.Modules.People.Application.DTOs;
using BlazorModularMonolith.Api.Shared.Common;

namespace BlazorModularMonolith.Api.Modules.People.Application.Services;

public interface IPersonService
{
    Task<Result<PersonDto>> GetPersonAsync(Guid id);
    Task<Result<IEnumerable<PersonDto>>> GetAllPeopleAsync();
    Task<Result<PersonDto>> GetPersonByEmailAsync(string email);
    Task<Result<PersonDto>> CreatePersonAsync(CreatePersonRequest request);
    Task<Result<PersonDto>> UpdatePersonAsync(Guid id, UpdatePersonRequest request);
    Task<Result> DeletePersonAsync(Guid id);
    Task<Result<PersonDto>> AddAddressToPersonAsync(Guid personId, Guid addressId);
    Task<Result<PersonDto>> RemoveAddressFromPersonAsync(Guid personId, Guid addressId);
}
