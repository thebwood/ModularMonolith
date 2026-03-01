using BlazorModularMonolith.Api.Modules.People.Domain.Entities;

namespace BlazorModularMonolith.Api.Modules.People.Domain.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id);
    Task<IEnumerable<Person>> GetAllAsync();
    Task<Person?> GetByEmailAsync(string email);
    Task<Person> CreateAsync(Person person);
    Task<Person?> UpdateAsync(Person person);
    Task<bool> DeleteAsync(Guid id);
}
