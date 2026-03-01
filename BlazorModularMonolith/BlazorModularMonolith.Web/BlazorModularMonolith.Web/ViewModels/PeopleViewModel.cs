using BlazorModularMonolith.Web.Models;
using BlazorModularMonolith.Web.Services;

namespace BlazorModularMonolith.Web.ViewModels;

public class PeopleViewModel : ViewModelBase
{
    private readonly IPersonApiService _personService;
    private readonly IAddressApiService _addressService;
    private readonly ILogger<PeopleViewModel> _logger;

    public PeopleViewModel(
        IPersonApiService personService, 
        IAddressApiService addressService,
        ILogger<PeopleViewModel> logger)
    {
        _personService = personService;
        _addressService = addressService;
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

    // Address Management Properties
    private List<AddressModel> _personAddresses = new();
    public List<AddressModel> PersonAddresses
    {
        get => _personAddresses;
        set => SetProperty(ref _personAddresses, value);
    }

    private List<AddressModel> _availableAddresses = new();
    public List<AddressModel> AvailableAddresses
    {
        get => _availableAddresses;
        set => SetProperty(ref _availableAddresses, value);
    }

    private bool _isManagingAddresses;
    public bool IsManagingAddresses
    {
        get => _isManagingAddresses;
        set => SetProperty(ref _isManagingAddresses, value);
    }

    private bool _isCreatingAddress;
    public bool IsCreatingAddress
    {
        get => _isCreatingAddress;
        set => SetProperty(ref _isCreatingAddress, value);
    }

    public CreateAddressModel NewAddress { get; set; } = new();

    // Address Management Methods
    public async Task StartManageAddressesAsync(PersonModel person)
    {
        try
        {
            SelectedPerson = person;
            IsBusy = true;
            ClearError();

            // Load person's addresses
            PersonAddresses = await _addressService.GetByOwnerIdAsync(person.Id);

            // Load all addresses to show available ones
            var allAddresses = await _addressService.GetAllAsync();
            AvailableAddresses = allAddresses
                .Where(a => !person.AddressIds.Contains(a.Id))
                .ToList();

            IsManagingAddresses = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading addresses for person {PersonId}", person.Id);
            SetError("Failed to load addresses. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> AddExistingAddressAsync(Guid addressId)
    {
        if (SelectedPerson == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            var updated = await _personService.AddAddressAsync(SelectedPerson.Id, addressId);
            if (updated != null)
            {
                // Update the person in the list
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = updated;
                    People = new List<PersonModel>(People);
                }

                // Refresh addresses
                await StartManageAddressesAsync(updated);
                return true;
            }

            SetError("Failed to add address to person.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address to person");
            SetError("Failed to add address. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> RemoveAddressAsync(Guid addressId)
    {
        if (SelectedPerson == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            var updated = await _personService.RemoveAddressAsync(SelectedPerson.Id, addressId);
            if (updated != null)
            {
                // Update the person in the list
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = updated;
                    People = new List<PersonModel>(People);
                }

                // Refresh addresses
                await StartManageAddressesAsync(updated);
                return true;
            }

            SetError("Failed to remove address from person.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing address from person");
            SetError("Failed to remove address. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void StartCreateAddress()
    {
        if (SelectedPerson == null) return;

        NewAddress = new CreateAddressModel
        {
            Type = AddressType.Person,
            OwnerId = SelectedPerson.Id,
            OwnerName = SelectedPerson.FullName
        };
        IsCreatingAddress = true;
    }

    public async Task<bool> CreateAndAddAddressAsync()
    {
        if (SelectedPerson == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            // Ensure the address is correctly configured
            NewAddress.Type = AddressType.Person;
            NewAddress.OwnerId = SelectedPerson.Id;
            NewAddress.OwnerName = SelectedPerson.FullName;

            // Create the address
            var createdAddress = await _addressService.CreateAsync(NewAddress);

            // Add it to the person
            var updated = await _personService.AddAddressAsync(SelectedPerson.Id, createdAddress.Id);
            if (updated != null)
            {
                // Update the person in the list
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = updated;
                    People = new List<PersonModel>(People);
                }

                // Reset form and refresh
                NewAddress = new CreateAddressModel();
                IsCreatingAddress = false;
                await StartManageAddressesAsync(updated);
                return true;
            }

            SetError("Failed to add newly created address to person.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating and adding address");
            SetError("Failed to create address. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void CancelAddressManagement()
    {
        IsManagingAddresses = false;
        IsCreatingAddress = false;
        SelectedPerson = null;
        PersonAddresses = new List<AddressModel>();
        AvailableAddresses = new List<AddressModel>();
        NewAddress = new CreateAddressModel();
        ClearError();
    }
}
