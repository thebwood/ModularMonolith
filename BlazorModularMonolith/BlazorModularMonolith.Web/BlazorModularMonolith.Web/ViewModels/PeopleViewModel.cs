using BlazorModularMonolith.Web.Models;
using BlazorModularMonolith.Web.Services;

namespace BlazorModularMonolith.Web.ViewModels;

public class PeopleViewModel : ViewModelBase
{
    private readonly IPersonApiService _personService;
    private readonly ILogger<PeopleViewModel> _logger;

    public PeopleViewModel(IPersonApiService personService, ILogger<PeopleViewModel> logger)
    {
        _personService = personService;
        _logger = logger;
    }

    private List<PersonModel> _people = new();
    public List<PersonModel> People
    {
        get => _people;
        set => SetProperty(ref _people, value);
    }

    private PersonModel? _selectedPerson;
    public PersonModel? SelectedPerson
    {
        get => _selectedPerson;
        set => SetProperty(ref _selectedPerson, value);
    }

    private bool _isCreating;
    public bool IsCreating
    {
        get => _isCreating;
        set => SetProperty(ref _isCreating, value);
    }

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public CreatePersonModel NewPerson { get; set; } = new();
    public CreatePersonModel EditPerson { get; set; } = new();

    public async Task LoadPeopleAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();
            People = await _personService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading people");
            SetError("Failed to load people. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> CreatePersonAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();
            var created = await _personService.CreateAsync(NewPerson);
            People.Add(created);
            People = new List<PersonModel>(People);
            NewPerson = new CreatePersonModel();
            IsCreating = false;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            SetError("Failed to create person. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void StartEdit(PersonModel person)
    {
        SelectedPerson = person;
        EditPerson = new CreatePersonModel
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            PhoneNumber = person.PhoneNumber,
            DateOfBirth = person.DateOfBirth
        };
        IsEditing = true;
    }

    public async Task<bool> UpdatePersonAsync()
    {
        if (SelectedPerson == null) return false;

        try
        {
            IsBusy = true;
            ClearError();
            var updated = await _personService.UpdateAsync(SelectedPerson.Id, EditPerson);
            if (updated != null)
            {
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = updated;
                    People = new List<PersonModel>(People);
                }
                IsEditing = false;
                SelectedPerson = null;
                return true;
            }
            SetError("Person not found.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person");
            SetError("Failed to update person. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> DeletePersonAsync(Guid id)
    {
        try
        {
            IsBusy = true;
            ClearError();
            var success = await _personService.DeleteAsync(id);
            if (success)
            {
                People = People.Where(p => p.Id != id).ToList();
                return true;
            }
            SetError("Failed to delete person.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person");
            SetError("Failed to delete person. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void CancelEdit()
    {
        IsEditing = false;
        IsCreating = false;
        SelectedPerson = null;
        EditPerson = new CreatePersonModel();
        NewPerson = new CreatePersonModel();
        ClearError();
    }
}
