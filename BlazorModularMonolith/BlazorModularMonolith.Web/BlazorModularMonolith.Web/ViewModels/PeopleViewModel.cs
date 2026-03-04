using BlazorModularMonolith.Web.Common;
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
            var result = await _personService.GetAllAsync();
            if (result.IsSuccess)
                People = result.Value!;
            else
                SetError(result.Error!);
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
            var result = await _personService.CreateAsync(NewPerson);
            if (result.IsSuccess)
            {
                People.Add(result.Value!);
                People = new List<PersonModel>(People);
                NewPerson = new CreatePersonModel();
                IsCreating = false;
                return true;
            }
            SetError(result.Error!);
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
            var result = await _personService.UpdateAsync(SelectedPerson.Id, EditPerson);
            if (result.IsSuccess)
            {
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = result.Value!;
                    People = new List<PersonModel>(People);
                }
                IsEditing = false;
                SelectedPerson = null;
                return true;
            }
            SetError(result.Error!);
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
            var result = await _personService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                People = People.Where(p => p.Id != id).ToList();
                return true;
            }
            SetError(result.Error!);
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

            var addressesResult = await _addressService.GetByOwnerIdAsync(person.Id);
            if (!addressesResult.IsSuccess)
            {
                SetError(addressesResult.Error!);
                return;
            }
            PersonAddresses = addressesResult.Value!;

            var allAddressesResult = await _addressService.GetAllAsync();
            if (!allAddressesResult.IsSuccess)
            {
                SetError(allAddressesResult.Error!);
                return;
            }
            AvailableAddresses = allAddressesResult.Value!
                .Where(a => !person.AddressIds.Contains(a.Id))
                .ToList();

            IsManagingAddresses = true;
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

            var result = await _personService.AddAddressAsync(SelectedPerson.Id, addressId);
            if (result.IsSuccess)
            {
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = result.Value!;
                    People = new List<PersonModel>(People);
                }
                await StartManageAddressesAsync(result.Value!);
                return true;
            }
            SetError(result.Error!);
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

            var result = await _personService.RemoveAddressAsync(SelectedPerson.Id, addressId);
            if (result.IsSuccess)
            {
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = result.Value!;
                    People = new List<PersonModel>(People);
                }
                await StartManageAddressesAsync(result.Value!);
                return true;
            }
            SetError(result.Error!);
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

            NewAddress.Type = AddressType.Person;
            NewAddress.OwnerId = SelectedPerson.Id;
            NewAddress.OwnerName = SelectedPerson.FullName;

            var addressResult = await _addressService.CreateAsync(NewAddress);
            if (!addressResult.IsSuccess)
            {
                SetError(addressResult.Error!);
                return false;
            }

            var personResult = await _personService.AddAddressAsync(SelectedPerson.Id, addressResult.Value!.Id);
            if (personResult.IsSuccess)
            {
                var index = People.FindIndex(p => p.Id == SelectedPerson.Id);
                if (index >= 0)
                {
                    People[index] = personResult.Value!;
                    People = new List<PersonModel>(People);
                }
                NewAddress = new CreateAddressModel();
                IsCreatingAddress = false;
                await StartManageAddressesAsync(personResult.Value!);
                return true;
            }
            SetError(personResult.Error!);
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
