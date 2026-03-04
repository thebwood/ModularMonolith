using BlazorModularMonolith.Web.Common;
using BlazorModularMonolith.Web.Models;
using BlazorModularMonolith.Web.Services;

namespace BlazorModularMonolith.Web.ViewModels;

public class BusinessesViewModel : ViewModelBase
{
    private readonly IBusinessApiService _businessService;
    private readonly IAddressApiService _addressService;
    private readonly ILogger<BusinessesViewModel> _logger;

    public BusinessesViewModel(
        IBusinessApiService businessService,
        IAddressApiService addressService,
        ILogger<BusinessesViewModel> logger)
    {
        _businessService = businessService;
        _addressService = addressService;
        _logger = logger;
    }

    private List<BusinessModel> _businesses = new();
    public List<BusinessModel> Businesses
    {
        get => _businesses;
        set => SetProperty(ref _businesses, value);
    }

    private BusinessModel? _selectedBusiness;
    public BusinessModel? SelectedBusiness
    {
        get => _selectedBusiness;
        set => SetProperty(ref _selectedBusiness, value);
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

    public CreateBusinessModel NewBusiness { get; set; } = new();
    public CreateBusinessModel EditBusiness { get; set; } = new();

    public async Task LoadBusinessesAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();
            var result = await _businessService.GetAllAsync();
            if (result.IsSuccess)
                Businesses = result.Value!;
            else
                SetError(result.Error!);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> CreateBusinessAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();
            var result = await _businessService.CreateAsync(NewBusiness);
            if (result.IsSuccess)
            {
                Businesses.Add(result.Value!);
                Businesses = new List<BusinessModel>(Businesses);
                NewBusiness = new CreateBusinessModel();
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

    public void StartEdit(BusinessModel business)
    {
        SelectedBusiness = business;
        EditBusiness = new CreateBusinessModel
        {
            Name = business.Name,
            TaxId = business.TaxId,
            Email = business.Email,
            PhoneNumber = business.PhoneNumber,
            Website = business.Website,
            Type = business.Type
        };
        IsEditing = true;
    }

    public async Task<bool> UpdateBusinessAsync()
    {
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();
            var result = await _businessService.UpdateAsync(SelectedBusiness.Id, EditBusiness);
            if (result.IsSuccess)
            {
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = result.Value!;
                    Businesses = new List<BusinessModel>(Businesses);
                }
                IsEditing = false;
                SelectedBusiness = null;
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

    public async Task<bool> DeleteBusinessAsync(Guid id)
    {
        try
        {
            IsBusy = true;
            ClearError();
            var result = await _businessService.DeleteAsync(id);
            if (result.IsSuccess)
            {
                Businesses = Businesses.Where(b => b.Id != id).ToList();
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
        SelectedBusiness = null;
        EditBusiness = new CreateBusinessModel();
        NewBusiness = new CreateBusinessModel();
        ClearError();
    }

    // Address Management Properties
    private List<AddressModel> _businessAddresses = new();
    public List<AddressModel> BusinessAddresses
    {
        get => _businessAddresses;
        set => SetProperty(ref _businessAddresses, value);
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
    public async Task StartManageAddressesAsync(BusinessModel business)
    {
        try
        {
            SelectedBusiness = business;
            IsBusy = true;
            ClearError();

            var addressesResult = await _addressService.GetByOwnerIdAsync(business.Id);
            if (!addressesResult.IsSuccess)
            {
                SetError(addressesResult.Error!);
                return;
            }
            BusinessAddresses = addressesResult.Value!;

            var allAddressesResult = await _addressService.GetAllAsync();
            if (!allAddressesResult.IsSuccess)
            {
                SetError(allAddressesResult.Error!);
                return;
            }
            AvailableAddresses = allAddressesResult.Value!
                .Where(a => !business.AddressIds.Contains(a.Id))
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
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            var result = await _businessService.AddAddressAsync(SelectedBusiness.Id, addressId);
            if (result.IsSuccess)
            {
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = result.Value!;
                    Businesses = new List<BusinessModel>(Businesses);
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
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            var result = await _businessService.RemoveAddressAsync(SelectedBusiness.Id, addressId);
            if (result.IsSuccess)
            {
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = result.Value!;
                    Businesses = new List<BusinessModel>(Businesses);
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
        if (SelectedBusiness == null) return;

        NewAddress = new CreateAddressModel
        {
            Type = AddressType.Business,
            OwnerId = SelectedBusiness.Id,
            OwnerName = SelectedBusiness.Name
        };
        IsCreatingAddress = true;
    }

    public async Task<bool> CreateAndAddAddressAsync()
    {
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            NewAddress.Type = AddressType.Business;
            NewAddress.OwnerId = SelectedBusiness.Id;
            NewAddress.OwnerName = SelectedBusiness.Name;

            var addressResult = await _addressService.CreateAsync(NewAddress);
            if (!addressResult.IsSuccess)
            {
                SetError(addressResult.Error!);
                return false;
            }

            var businessResult = await _businessService.AddAddressAsync(SelectedBusiness.Id, addressResult.Value!.Id);
            if (businessResult.IsSuccess)
            {
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = businessResult.Value!;
                    Businesses = new List<BusinessModel>(Businesses);
                }
                NewAddress = new CreateAddressModel();
                IsCreatingAddress = false;
                await StartManageAddressesAsync(businessResult.Value!);
                return true;
            }
            SetError(businessResult.Error!);
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
        SelectedBusiness = null;
        BusinessAddresses = new List<AddressModel>();
        AvailableAddresses = new List<AddressModel>();
        NewAddress = new CreateAddressModel();
        ClearError();
    }
}
